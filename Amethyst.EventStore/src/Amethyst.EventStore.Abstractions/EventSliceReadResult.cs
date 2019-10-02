using System.Collections.Generic;

namespace Amethyst.EventStore.Abstractions
{
    public readonly struct EventSliceReadResult
    {
        public EventSliceReadResult(
            ReadStatus status,
            StreamId streamId,
            IReadOnlyCollection<RecordedEvent> events,
            long lastEventNumber,
            bool isEndOfStream)
        {
            Status = status;
            StreamId = streamId;
            Events = events;
            LastEventNumber = lastEventNumber;
            IsEndOfStream = isEndOfStream;
        }
        
        public ReadStatus Status { get; }
        public StreamId StreamId { get; }
        public IReadOnlyCollection<RecordedEvent> Events { get; }
        public long LastEventNumber { get; }
        public bool IsEndOfStream { get; }
    }
}