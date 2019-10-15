using System.Collections.Generic;

namespace Amethyst.EventStore
{
    public readonly struct SliceReadResult<T>
    {
        public readonly ReadStatus Status;
        public readonly StreamId Stream;
        public readonly IReadOnlyCollection<T> Events;
        public readonly long LastEventNumber;
        public readonly bool IsEndOfStream;

        public SliceReadResult(
            ReadStatus status,
            StreamId stream,
            IReadOnlyCollection<T> events,
            long lastEventNumber,
            bool isEndOfStream)
        {
            Status = status;
            Stream = stream;
            Events = events;
            LastEventNumber = lastEventNumber;
            IsEndOfStream = isEndOfStream;
        }
    }
}