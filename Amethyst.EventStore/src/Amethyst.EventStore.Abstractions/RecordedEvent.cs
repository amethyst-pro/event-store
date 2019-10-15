using System;

namespace Amethyst.EventStore.Abstractions
{
    public sealed class RecordedEvent
    {
        public readonly StreamId StreamId;
        public readonly Guid Id;
        public readonly long Number;
        public readonly string Type;
        public readonly DateTime Created;
        public readonly long CreatedEpoch;
        public readonly byte[] Data;
        public readonly byte[] Metadata;

        public RecordedEvent(
            StreamId streamId,
            Guid id,
            long number,
            string type,
            DateTime created,
            long createdEpoch,
            byte[] data,
            byte[] metadata)
        {
            StreamId = streamId;
            Id = id;
            Number = number;
            Type = type;
            Created = created;
            CreatedEpoch = createdEpoch;
            Data = data;
            Metadata = metadata;
        }
    }
}