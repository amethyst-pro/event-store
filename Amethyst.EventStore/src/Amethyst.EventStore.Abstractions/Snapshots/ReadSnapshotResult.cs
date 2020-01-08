using System;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public readonly struct ReadSnapshotResult<T>
    {
        public T Snapshot { get; }
        public long EventNumber { get; }
        public DateTimeOffset CreatedAt { get; }

        public ReadSnapshotResult(T snapshot, long eventNumber, DateTimeOffset createdAt)
        {
            Snapshot = snapshot;
            EventNumber = eventNumber;
            CreatedAt = createdAt;
        }
    }
}