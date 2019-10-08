using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Storage;
using Npgsql;

namespace Amethyst.EventStore.Postgres
{
   public sealed class EventsReader
    {
        private static int _connectionCounter;
        
        private readonly DbConnections _connections;
        private readonly IEventStoreContext _context;
        private readonly IEventTypeRegistry _typeRegistry;

        public EventsReader(
            DbConnections settings,
            IEventStoreContext context,
            IEventTypeRegistry typeRegistry)
        {
            _connections = settings ?? throw new ArgumentNullException(nameof(settings));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
        }

        public async Task<EventSliceReadResult> ReadStreamEventsForwardAsync(StreamId stream, long start, int count)
        {
            if (start < StreamPosition.End)
                throw new ArgumentOutOfRangeException(nameof(start));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            using (var connection = GetConnection())
            {
                await connection.OpenWithSchemaAsync(_context.GetSchema(stream));

                return await ReadStreamEventsForwardAsync(stream, start, count, connection);
            }
        }

        public async Task<EventSliceReadResult> ReadStreamEventsForwardAsync(
            StreamId stream,
            long start,
            int count,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction = null)
        {
            using (var command = BuildCommand(stream, start, count, connection, transaction))
            {
                await command.PrepareAsync();

                var commandWithDiagnostics = command; //.WithDiagnostics();

                for (var attempt = 1;; ++attempt)
                {
                    try
                    {
                        using (var reader = await commandWithDiagnostics.ExecuteReaderAsync())
                            return await ReadEventsSlice(stream, reader);
                    }
                    catch (InconsistentReadException)
                    {
                        if (attempt > 15)
                            throw new InvalidOperationException("Inconsistent read attempts exceeded.");
                    }
                }
            }
        }

        internal async Task<long> GetLastSentEventNumber(
            StreamId stream,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction = null)
        {
            using (var getLastSentEvent = BuildGetLastSentCommand(stream, connection, transaction))
            {
                await getLastSentEvent.PrepareAsync();

                return (long) await getLastSentEvent.ExecuteScalarAsync();
            }
        }

        private static NpgsqlCommand BuildGetLastSentCommand(
            StreamId stream,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            const string prepare = @"SELECT last_sent_event_number FROM streams where id = @streamId";

            var command = new NpgsqlCommand(prepare, connection, transaction)
            {
                Parameters =
                {
                    new NpgsqlParameter<Guid>("streamId", stream.Id)
                }
            };

            return command;
        }

        private async Task<EventSliceReadResult> ReadEventsSlice(StreamId stream, DbDataReader reader)
        {
            var hasStream = await reader.ReadAsync();

            if (!hasStream)
                return Empty(ReadStatus.NoStream, stream);

            var streamLastEventNumber = reader.GetInt64(0);

            await reader.NextResultAsync();

            var events = await ReadEvents(stream, reader);

            if (events.Count == 0)
                return Empty(ReadStatus.NotFound, stream);

            var lastEventNumber = events.Last().EventNumber;

            if (streamLastEventNumber < lastEventNumber)
                throw new InconsistentReadException();

            return new EventSliceReadResult(
                ReadStatus.Success,
                stream,
                events,
                streamLastEventNumber,
                streamLastEventNumber == lastEventNumber);
        }

        private NpgsqlCommand BuildCommand(
            StreamId stream,
            long start,
            int count,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            const string selectHeader = @"
                    SELECT last_event_number FROM streams WHERE id = @streamId;
                    SELECT event_id, number, type_id, created, data, metadata 
                        FROM events
                        WHERE stream_id = @streamId ";

            const string selectHeaderForward = selectHeader + " AND number >= @start ORDER BY id LIMIT @count;";

            const string selectHeaderLast = selectHeader + " ORDER BY id DESC LIMIT 1;";

            var readLast = start < 0;

            var select = (readLast
                ? selectHeaderLast
                : selectHeaderForward);

            var command = new NpgsqlCommand(select, connection, transaction)
            {
                Parameters =
                {
                    new NpgsqlParameter<Guid>("streamId", stream.Id),
                    new NpgsqlParameter<long>("start", start),
                    new NpgsqlParameter<int>("count", count),
                }
            };

            return command;
        }

        private async Task<IReadOnlyCollection<RecordedEvent>> ReadEvents(StreamId stream, DbDataReader reader)
        {
            var events = new List<RecordedEvent>();

            while (await reader.ReadAsync())
            {
                var created = reader.GetDateTime(3);

                events.Add(new RecordedEvent(
                    stream,
                    id: reader.GetGuid(0),
                    number: reader.GetInt64(1),
                    type: _typeRegistry.GetTypeName(reader.GetInt16(2), stream),
                    created,
                    data: reader.GetFieldValue<byte[]>(4),
                    metadata: reader.GetFieldValue<byte[]>(5)));
            }

            return events;
        }

        private static EventSliceReadResult Empty(ReadStatus status, StreamId stream)
        {
            return new EventSliceReadResult(
                status,
                stream,
                Array.Empty<RecordedEvent>(), 0, true);
        }
        
        private NpgsqlConnection GetConnection()
        {
            var connection = _connections.ReadOnly;
            if (Interlocked.Increment(ref _connectionCounter) % 3 == 0)
                connection = _connections.Default;

            return new NpgsqlConnection(connection);
        }
    }
}