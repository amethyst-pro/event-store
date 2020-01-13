using System;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Streams.Abstractions;

namespace Amethyst.EventStore.Streams
{
    public sealed class StreamReader : IStreamReader
    {
        private readonly IEventsReader<StoredEvent> _eventsReader;

        public StreamReader(IEventsReader<StoredEvent> eventsReader)
        {
            _eventsReader = eventsReader;
        }

        public Task<SliceReadResult<StoredEvent>> ReadEventsForward(StreamId stream, long start,
            int count = int.MaxValue)
        {
            return _eventsReader.ReadEventsForward(stream, start, count);
        }

        public async Task<ReadResult<StoredEvent>> ReadEvent(StreamId stream, long eventNumber)
        {
            var eventsReadResult = await _eventsReader.ReadEventsForward(stream, eventNumber, count: 1);

            switch (eventsReadResult.Status)
            {
                case ReadStatus.Success:
                    return new ReadResult<StoredEvent>(ReadStatus.Success, stream,
                        eventsReadResult.Events.Single());
                case ReadStatus.NotFound:
                    return new ReadResult<StoredEvent>(ReadStatus.NotFound, stream, default);
                case ReadStatus.NoStream:
                    return new ReadResult<StoredEvent>(ReadStatus.NoStream, stream, default);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}