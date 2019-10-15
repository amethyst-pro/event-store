using System;

namespace Amethyst.EventStore
{
    public readonly struct EventData
    {
        public readonly Guid EventId;
        public readonly string Type;
        public readonly byte[] Data;
        public readonly byte[] Metadata;

        public EventData(Guid eventId, string type, byte[] data, byte[] metadata)
        {
            this.EventId = eventId;
            this.Type = type;
            this.Data = data ?? Array.Empty<byte>();
            this.Metadata = metadata ?? Array.Empty<byte>();
        }
    }
}