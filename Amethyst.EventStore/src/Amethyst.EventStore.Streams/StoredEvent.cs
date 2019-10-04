using System;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Streams
{
    public sealed class StoredEvent
    {
        public StoredEvent(
            StreamId streamId,
            Guid id,
            long number,
            string type,
            DateTime created,
            IDomainEvent data,
            object metadata = null)
        {
            StreamId = streamId;
            Id = id;
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

        public IDomainEvent Data { get; }
        
        public object Metadata { get; }
    }
}