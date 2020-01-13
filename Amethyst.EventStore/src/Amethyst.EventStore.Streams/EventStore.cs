using System.Collections.Generic;
using System.Threading.Tasks;
using Amethyst.EventStore.Streams.Abstractions;

namespace Amethyst.EventStore.Streams
{
    public sealed class EventStore : IEventStore
    {
        private readonly IStreamReader _reader;
        private readonly IStreamWriter _writer;

        public EventStore(IStreamReader reader, IStreamWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public Task<SliceReadResult<StoredEvent>> ReadEventsForward(
            StreamId stream,
            long start,
            int count = int.MaxValue)
            => _reader.ReadEventsForward(stream, start, count);

        public Task<ReadResult<StoredEvent>> ReadEvent(StreamId stream, long eventNumber)
            => _reader.ReadEvent(stream, eventNumber);

        public Task<WriteResult> Append(
            StreamId stream,
            long expectedVersion,
            IEnumerable<StreamEvent> events)
            => _writer.Append(stream, expectedVersion, events);
    }
}