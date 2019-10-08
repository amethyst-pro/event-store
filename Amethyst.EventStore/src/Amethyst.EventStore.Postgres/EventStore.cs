using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;

namespace Amethyst.EventStore.Postgres
{
    public sealed class EventStore : IEventStore
    {
        private readonly EventsReader _reader;
        private readonly EventsWriter _writer;

        public EventStore(EventsReader reader, EventsWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public Task<EventSliceReadResult> ReadStreamEventsForwardAsync(
            StreamId stream, 
            long start,
            int count = int.MaxValue)
            => _reader.ReadStreamEventsForwardAsync(stream, start, count);

        public async Task<EventReadResult> ReadEventAsync(StreamId stream, long eventNumber)
        {
            var readResult = await _reader.ReadStreamEventsForwardAsync(stream, eventNumber, count: 1);

            switch (readResult.Status)
            {
                case ReadStatus.Success:
                    return new EventReadResult(ReadStatus.Success, stream, readResult.Events.Single());
                case ReadStatus.NotFound:
                    return new EventReadResult(ReadStatus.NotFound, stream, default);
                case ReadStatus.NoStream:
                    return new EventReadResult(ReadStatus.NoStream, stream, default);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Task<WriteResult> AppendToStreamAsync(
            StreamId stream, long expectedVersion, IEnumerable<EventData> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            return _writer.AppendToStreamAsync(stream, expectedVersion, events.ToArray());
        }
    }
}