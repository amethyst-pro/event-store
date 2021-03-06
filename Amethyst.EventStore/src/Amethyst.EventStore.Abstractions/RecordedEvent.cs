using System;

namespace Amethyst.EventStore.Abstractions
{
    public sealed class RecordedEvent
    {
        public RecordedEvent(
            StreamId streamId,
            Guid id,
            long number,
            string type,
            DateTime created,
            byte[] data,
            byte[] metadata)
        {
            StreamId = streamId;
            this.Id = id;
            Number = number;
            Type = type;
            Created = created;
            Data = data;
            Metadata = metadata;
        }
        
        public StreamId StreamId { get; }
        
        public Guid Id { get; }
        
        public long Number { get; }
        
        public string Type { get; }
        
        public DateTime Created { get; }
        
        public byte[] Data { get; }
        
        public byte[] Metadata { get; }
    }
}