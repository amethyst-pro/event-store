using System;
using SharpJuice.Essentials;

namespace Amethyst.EventStore
{
    public readonly struct SnapshotableReadResult<TEvent, TSnapshot>
    {
        public readonly ReadStatus Status;
        public readonly StreamId Stream;
        public readonly TEvent[] HeadEvents;
        public readonly Maybe<TSnapshot> Snapshot;
        public readonly long LastEventNumber;

        public static SnapshotableReadResult<TEvent, TSnapshot> Empty(
            ReadStatus status,
            StreamId stream)
        {
            return new SnapshotableReadResult<TEvent, TSnapshot>(
                status, stream, Array.Empty<TEvent>(), 0, default);
        }

        public SnapshotableReadResult(
            ReadStatus status,
            StreamId stream,
            TEvent[] headEvents,
            long lastEventNumber,
            Maybe<TSnapshot> snapshot)
        {
            Status = status;
            Stream = stream;
            HeadEvents = headEvents ?? Array.Empty<TEvent>();
            LastEventNumber = lastEventNumber;
            Snapshot = snapshot;
        }
    }
}