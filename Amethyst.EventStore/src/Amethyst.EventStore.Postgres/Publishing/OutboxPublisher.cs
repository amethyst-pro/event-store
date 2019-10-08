using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Storage;
using Npgsql;

namespace Amethyst.EventStore.Postgres.Publishing
{
    public sealed class OutboxPublisher
    {
        private const int MaxStreamsPerRead = 10;

        private readonly DbConnections _connections;
        private readonly IReadOnlyDictionary<string, IEventStoreContext> _categories;
        private readonly EventsReader _reader;

        public OutboxPublisher(
            DbConnections connections,
            IReadOnlyDictionary<string, IEventStoreContext> categories,
            EventsReader reader)
        {
            _connections = connections;
            _categories = categories;
            _reader = reader;
        }

        public async Task PublishAsync()
        {
            var exceptions = new ConcurrentBag<Exception>();

            var publishBlock = new ActionBlock<EventStoreSchema>(
                Publish,
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                });

            foreach (var (category, context) in _categories)
            foreach (var schema in context.GetSchemas(category))
            {
                publishBlock.Post(new EventStoreSchema(category, schema, context));
            }

            publishBlock.Complete();

            await publishBlock.Completion;

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            async Task Publish(EventStoreSchema schema)
            {
                using (var connection = new NpgsqlConnection(_connections.Default))
                {
                    await connection.OpenWithSchemaAsync(schema.Schema);

                    try
                    {
                        await SendIfAny(schema.Category, schema.Context, connection);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
        }

        private async Task SendIfAny(
            string category,
            IEventStoreContext context,
            NpgsqlConnection connection)
        {
            bool hasMoreStreams;

            do
            {
                var streamsInOutbox = await GetTopOfOutboxStreams(connection);
                hasMoreStreams = streamsInOutbox.Count == MaxStreamsPerRead;

                foreach (var id in streamsInOutbox)
                {
                    var stream = new StreamId(category, id);
                    var lockId = stream.GetLockId();

                    if (await TryLock(lockId, connection))
                    {
                        var events = await GetEvents(stream, connection);

                        if (events.Count == 0)
                        {
                            await Unlock(lockId, connection);
                            continue;
                        }

                        var operation = new EventSendingOperation(
                            stream,
                            lockId,
                            events,
                            connection,
                            context.GetPublisher(stream));

                        await operation.SendAsync();
                    }
                }
            } while (hasMoreStreams);
        }

        private async Task<IReadOnlyCollection<RecordedEvent>> GetEvents(StreamId stream,
            NpgsqlConnection connection)
        {
            var lastSentEventNumber = await _reader.GetLastSentEventNumber(
                stream, connection);

            var startSendFrom = lastSentEventNumber + 1;

            var readResult = await _reader.ReadStreamEventsForwardAsync(
                stream, startSendFrom, int.MaxValue, connection);

            if (readResult.Status == ReadStatus.NotFound) //concurrent sent happened
                return Array.Empty<RecordedEvent>();

            return readResult.Events;
        }

        private static async Task<bool> TryLock(long lockId, NpgsqlConnection connection)
        {
            const string tryLock = "SELECT pg_try_advisory_lock(@lock)";

            using (var command = new NpgsqlCommand(tryLock, connection))
            {
                command.Parameters.Add(new NpgsqlParameter<long>("lock", lockId));

                await command.PrepareAsync();

                return (bool) await command.ExecuteScalarAsync();
            }
        }

        private static async Task Unlock(long lockId, NpgsqlConnection connection)
        {
            const string unlock = "SELECT pg_advisory_unlock(@lock);";

            using (var command = new NpgsqlCommand(unlock, connection))
            {
                command.Parameters.Add(new NpgsqlParameter<long>("lock", lockId));

                await command.PrepareAsync();

                await command.ExecuteScalarAsync();
            }
        }

        private async Task<IReadOnlyCollection<Guid>> GetTopOfOutboxStreams(NpgsqlConnection connection)
        {
            var select = $"SELECT stream_id FROM outbox ORDER BY RANDOM() LIMIT {MaxStreamsPerRead}";
            using (var command = new NpgsqlCommand(select, connection))
            {
                await command.PrepareAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var ids = new List<Guid>();
                    while (await reader.ReadAsync())
                    {
                        ids.Add(reader.GetGuid(0));
                    }

                    return ids;
                }
            }
        }

        private readonly struct EventStoreSchema
        {
            public EventStoreSchema(string category, string schema, IEventStoreContext context)
            {
                Category = category;
                Schema = schema;
                Context = context;
            }

            public string Category { get; }
            public string Schema { get; }
            public IEventStoreContext Context { get; }
        }
    }
}