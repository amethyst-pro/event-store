using System;

namespace Amethyst.EventStore.Abstractions
{
    public readonly struct EventData
    {
        public EventData(Guid eventId, string type, byte[] data, byte[] metadata)
        {
            Id = eventId;
            Type = type;
            Data = data ?? Array.Empty<byte>();
            Metadata = metadata ?? Array.Empty<byte>();
        }
        
        public Guid Id { get; }
        
        public string Type { get; }
        
        public byte[] Data { get; }
        
        public byte[] Metadata { get; }
    }
}