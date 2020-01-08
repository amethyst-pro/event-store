using System;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public readonly struct SnapshotData
    {
        public StreamId Id { get; }
        public long EventNumber { get; }
        public byte[] Data { get; }
        public DateTimeOffset CreatedAt { get; }

        public SnapshotData(StreamId id, long eventNumber, byte[] data, DateTimeOffset createdAt)
        {
            Id = id;
            EventNumber = eventNumber;
            Data = data;
            CreatedAt = createdAt;
        }
    }
}