using System;

namespace Amethyst.EventStore.Abstractions
{
    public readonly struct EventHeader
    {
        public readonly Guid Id;
        public readonly long Number;
        public readonly string Type;
        public readonly DateTime Created;

        public EventHeader(
            Guid id,
            long number,
            string type,
            DateTime created)
        {
            Id = id;
            Number = number;
            Type = type;
            Created = created;
        }
    }
}