using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Npgsql;

namespace Amethyst.EventStore.Postgres.Reading
{
    public sealed class EventsReader<T> : IEventsReader<T>
    {
        private static int _connectionCounter;

        private readonly PgsqlConnections _connections;
        private readonly IEventStoreContext _context;
        private readonly IDbEventReader<T> _eventDbReader;

        public EventsReader(
            PgsqlConnections settings,
            IEventStoreContext context,
            IDbEventReader<T> eventDbReader)
        {
            _connections = settings ?? throw new ArgumentNullException(nameof(settings));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventDbReader = eventDbReader;
        }

        public async Task<SliceReadResult<T>> ReadEventsForwardAsync(StreamId stream, long start, int count)
        {
            if (start < StreamPosition.End)
                throw new ArgumentOutOfRangeException(nameof(start));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            using (var connection = GetConnection())
            {
                await connection.OpenWithSchemaAsync(_context.GetSchema(stream));

                return await ReadEventsForwardAsync(stream, start, count, connection);
            }
        }

        public async Task<SliceReadResult<T>> ReadEventsForwardAsync(
            StreamId stream,
            long start,
            int count,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction = null)
        {
            using (var command = BuildCommand(stream, start, count, connection, transaction))
            {
                await command.PrepareAsync();

                for (var attempt = 1;; ++attempt)
                {
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
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

        private async Task<SliceReadResult<T>> ReadEventsSlice(StreamId stream, DbDataReader reader)
        {
            var hasStream = await reader.ReadAsync();

            if (!hasStream)
                return Empty(ReadStatus.NoStream, stream);

            var streamLastEventNumber = reader.GetInt64(0);

            await reader.NextResultAsync();

            var result = await _eventDbReader.Read(stream, reader);

            if (result.Events.Count == 0)
                return Empty(ReadStatus.NotFound, stream);

            if (streamLastEventNumber < result.LastEventNumber)
                throw new InconsistentReadException();

            return new SliceReadResult<T>(
                ReadStatus.Success,
                stream,
                result.Events,
                streamLastEventNumber,
                streamLastEventNumber == result.LastEventNumber);
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
                    SELECT event_id, number, type_id, created, metadata, data 
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

        private static SliceReadResult<T> Empty(ReadStatus status, StreamId stream)
        {
            return new SliceReadResult<T>(
                status,
                stream,
                Array.Empty<T>(), 0, true);
        }

        //TODO: move to connection factory
        private NpgsqlConnection GetConnection()
        {
            var connection = _connections.ReadOnly;
            if (Interlocked.Increment(ref _connectionCounter) % 3 == 0)
                connection = _connections.Default;

            return new NpgsqlConnection(connection);
        }
    }
}