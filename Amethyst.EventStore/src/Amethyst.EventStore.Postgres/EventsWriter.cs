using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Abstractions.Storage;
using Amethyst.EventStore.Postgres.Publishing;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace Amethyst.EventStore.Postgres
{
    public sealed class EventsWriter
    {
        private readonly DbConnections _connections;
        private readonly IEventStoreContext _context;
        private readonly IEventTypeRegistry _typeRegistry;
        private readonly EventsOutbox _outbox;
        private readonly ILogger<EventsWriter> _logger;

        public EventsWriter(
            DbConnections connections,
            IEventStoreContext context,
            IEventTypeRegistry typeRegistry,
            EventsOutbox outbox,
            ILogger<EventsWriter> logger)
        {
            _connections = connections;
            _context = context;
            _typeRegistry = typeRegistry;
            _outbox = outbox;
            _logger = logger;
        }

        public async Task<WriteResult> AppendToStreamAsync(
            StreamId stream,
            long expectedVersion,
            IReadOnlyCollection<EventData> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            if (expectedVersion < ExpectedVersion.Any)
                throw new ArgumentOutOfRangeException(nameof(expectedVersion));

            using (var connection = new NpgsqlConnection(_connections.Default))
            {
                await connection.OpenWithSchemaAsync(_context.GetSchema(stream));

                var (writeResult, sendingOperation) = await Append(stream, expectedVersion, events, connection);

                try
                {
                    await sendingOperation.SendAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception occured on sending event.");
                }

                return writeResult;
            }
        }

        private async Task<(WriteResult result, ISendingOperation sendingOperation)> Append(
            StreamId stream,
            long expectedVersion,
            IReadOnlyCollection<EventData> events,
            NpgsqlConnection connection)
        {
            await using var transaction = connection.BeginTransaction();
            var eventsCount = events.Count;

            var newLastEventNumber = await UpsertStream(stream, expectedVersion, eventsCount,
                connection, transaction);

            var prevLastEventNumber = newLastEventNumber - events.Count;

            var recordingEvents = GetRecordedEvents(stream, prevLastEventNumber, events).ToArray();

            await AddEvents(recordingEvents, connection, transaction);

            var sendingOperation = await _outbox.PrepareSendingAsync(
                stream, recordingEvents, connection, transaction);

            await transaction.CommitAsync();

            return (new WriteResult(newLastEventNumber), sendingOperation);
        }

        private async Task<long> UpsertStream(
            StreamId stream,
            long expectedVersion,
            int eventsCount,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            await using var streamUpsert = BuildUpsertStreamCommand(
                stream,
                expectedVersion,
                eventsCount,
                connection,
                transaction);
            await streamUpsert.PrepareAsync();

            var result = await streamUpsert.ExecuteScalarAsync();

            if (result == null)
                throw new WrongExpectedVersionException(
                    "Stream expected version doesn't match current.");

            return (long) result;
        }

        private NpgsqlCommand BuildUpsertStreamCommand(
            StreamId stream,
            long expectedVersion,
            int eventsCount,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            var command = new NpgsqlCommand
            {
                Parameters =
                {
                    new NpgsqlParameter<Guid>("streamId", stream.Id),
                    new NpgsqlParameter<int>("eventsCount", eventsCount),
                },
                Connection = connection,
                Transaction = transaction
            };

            if (expectedVersion == ExpectedVersion.NoStream)
            {
                command.CommandText =
                    @"INSERT INTO streams(id, last_event_number) VALUES(@streamId, @eventsCount - 1) 
                            ON CONFLICT (id) DO NOTHING
                            RETURNING last_event_number;";
            }
            else if (expectedVersion == ExpectedVersion.Any)
            {
                command.CommandText =
                    @"INSERT INTO streams(id, last_event_number) VALUES(@streamId, @eventsCount - 1) 
                            ON CONFLICT (id) DO UPDATE SET last_event_number = streams.last_event_number + @eventsCount 
                            RETURNING last_event_number;";
            }
            else
            {
                command.CommandText =
                    @"UPDATE streams SET last_event_number = last_event_number + @eventsCount 
                         WHERE id = @streamId AND last_event_number = @expectedVersion
                         RETURNING last_event_number;";

                command.Parameters.Add(new NpgsqlParameter<long>("expectedVersion", expectedVersion));
            }

            return command;
        }

        private IEnumerable<RecordedEvent> GetRecordedEvents(
            StreamId stream,
            long lastEventNumber,
            IReadOnlyCollection<EventData> events)
        {
            var created = DateTime.UtcNow;

            foreach (var e in events)
            {
                ++lastEventNumber;

                yield return new RecordedEvent(
                    stream,
                    e.Id,
                    lastEventNumber,
                    e.Type,
                    created,
                    e.Data,
                    e.Metadata);
            }
        }

        private async Task AddEvents(
            IReadOnlyCollection<RecordedEvent> events,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            var idParameter = new NpgsqlParameter<Guid>("eventId", NpgsqlDbType.Uuid);
            var streamIdParameter = new NpgsqlParameter<Guid>("streamId", NpgsqlDbType.Uuid);
            var numberParameter = new NpgsqlParameter<long>("number", NpgsqlDbType.Bigint);
            var typeParameter = new NpgsqlParameter<short>("typeId", NpgsqlDbType.Smallint);
            var dataParameter = new NpgsqlParameter<byte[]>("data", NpgsqlDbType.Bytea);
            var metadataParameter = new NpgsqlParameter<byte[]>("metadata", NpgsqlDbType.Bytea);

            const string insert = @"INSERT INTO events (event_id, stream_id, number, type_id, data, metadata)
                VALUES (@eventId, @streamId, @number, @typeId, @data, @metadata);";

            await using var insertCommand = new NpgsqlCommand(insert, connection, transaction);
            insertCommand.Parameters.AddRange(
                new NpgsqlParameter[]
                {
                    idParameter,
                    streamIdParameter,
                    numberParameter,
                    typeParameter,
                    dataParameter,
                    metadataParameter
                });

            await insertCommand.PrepareAsync();

            var commandWithDiagnostics = insertCommand /*.WithDiagnostics()*/;

            foreach (var e in events)
            {
                idParameter.TypedValue = e.Id;
                streamIdParameter.TypedValue = e.StreamId.Id;
                numberParameter.TypedValue = e.Number;
                typeParameter.TypedValue = _typeRegistry.GetOrAddTypeId(e.Type, e.StreamId);
                dataParameter.TypedValue = e.Data;
                metadataParameter.TypedValue = e.Metadata;

                await commandWithDiagnostics.ExecuteNonQueryAsync();
            }
        }
    }
}