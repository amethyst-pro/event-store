using System;

namespace Amethyst.EventStore.Abstractions
{
    public readonly struct EventHeader
    {
        public readonly StreamId StreamId;
        public readonly Guid Id;
        public readonly long Number;
        public readonly string Type;
        public readonly DateTime Created;
        
        public EventHeader(
            StreamId streamId,
            Guid id,
            long number,
            string type,
            DateTime created)
        {
            StreamId = streamId;
            Id = id;
            Number = number;
            Type = type;
            Created = created;
        }
    }
}