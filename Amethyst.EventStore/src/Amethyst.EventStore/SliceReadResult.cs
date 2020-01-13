using System;
using System.Collections.Generic;

namespace Amethyst.EventStore
{
    public readonly struct SliceReadResult<T>
    {
        public readonly ReadStatus Status;
        public readonly StreamId Stream;
        public readonly T[] Events;
        public readonly long LastEventNumber;
        public readonly bool IsEndOfStream;

        public static SliceReadResult<T> Empty(ReadStatus status, StreamId stream) =>
            new SliceReadResult<T>(status, stream, Array.Empty<T>(), 0, true);

        public SliceReadResult(
            ReadStatus status,
            StreamId stream,
            T[] events,
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