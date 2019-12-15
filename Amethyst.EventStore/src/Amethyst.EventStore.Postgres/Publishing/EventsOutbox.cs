using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Abstractions.Storage;
using Npgsql;

namespace Amethyst.EventStore.Postgres.Publishing
{
    public sealed class EventsOutbox
    {
        private readonly IEventStoreContext _context;
        private readonly EventsReader _reader;

        public EventsOutbox(IEventStoreContext context, EventsReader reader)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public async Task<ISendingOperation> PrepareSendingAsync(
            StreamId stream,
            IReadOnlyCollection<RecordedEvent> events,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            if (events.Count == 0)
                throw new ArgumentException("Events is empty", nameof(events));

            var streamLock = stream.GetLockId();
            long lastSentEventNumber;

            await using (var prepareSending = BuildPrepareCommand(stream, streamLock, connection, transaction))
            {
                await prepareSending.PrepareAsync();
                await using var reader = await prepareSending.ExecuteReaderAsync();
                
                await reader.NextResultAsync();
                await reader.ReadAsync();
                lastSentEventNumber = reader.GetInt64(0);
            }
            
            var startSendFrom = lastSentEventNumber + 1;

            if (events.First().Number == startSendFrom)
                return new SendingOperation(
                    stream, 
                    streamLock, 
                    events, 
                    connection,
                    _context.GetPublisher(stream));
            
            var readResult = await _reader.ReadStreamEventsForwardAsync(
                stream, 
                startSendFrom, 
                int.MaxValue, 
                connection, 
                transaction);

            ValidateReadResult(readResult, startSendFrom, events.Count);

            events = readResult.Events;

            return new SendingOperation(stream, streamLock, events, connection, _context.GetPublisher(stream));

            void ValidateReadResult(EventSliceReadResult result, long firstEventNumber, int prevEventsCount)
            {
                if (result.Status != ReadStatus.Success)
                    throw new InvalidOperationException($"Events read status is {result.Status}");

                if (result.Events.First().Number != firstEventNumber)
                    throw new InvalidOperationException(
                        $"First event must have number {firstEventNumber} but had {result.Events.First().Number}");

                if (result.Events.Count <= prevEventsCount)
                    throw new InvalidOperationException(
                        $"Expected events count must be greater then {prevEventsCount}");
            }
        }

        private NpgsqlCommand BuildPrepareCommand(
            StreamId stream,
            long streamLock,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            const string prepare = @"
                SELECT pg_advisory_lock(@lock);
                INSERT INTO outbox(stream_id) values(@streamId)
                    ON CONFLICT (stream_id) DO NOTHING;
                SELECT last_sent_event_number FROM streams where id = @streamId";

            var command = new NpgsqlCommand(prepare, connection, transaction)
            {
                Parameters =
                {
                    new NpgsqlParameter<long>("lock", streamLock),
                    new NpgsqlParameter<Guid>("streamId", stream.Id)
                }
            };

            return command;
        }
    }
}