using System;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public sealed class RecordedEvent
    {
        public RecordedEvent(
            StreamId streamId,
            Guid eventId,
            long eventNumber,
            string eventType,
            DateTime created,
            byte[] data,
            byte[] metadata)
        {
            StreamId = streamId;
            EventId = eventId;
            EventNumber = eventNumber;
            EventType = eventType;
            Created = created;
            Data = data;
            Metadata = metadata;
        }
        
        public StreamId StreamId { get; }
        
        public Guid EventId { get; }
        
        public long EventNumber { get; }
        
        public string EventType { get; }
        
        public DateTime Created { get; }
        
        public byte[] Data { get; }
        
        public byte[] Metadata { get; }
    }
}