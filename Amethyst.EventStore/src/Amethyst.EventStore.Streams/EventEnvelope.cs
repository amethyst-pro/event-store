using System;
using Amethyst.EventStore.Streams.Abstractions.Metadata;

namespace Amethyst.EventStore.Streams
{
    public readonly struct EventEnvelope<T>
    {
        public EventEnvelope(
            StreamId eventStreamId,
            long eventNumber,
            string eventType,
            DateTime created,
            T domainEvent,
            EventMetadata? metadata)
        {
            EventStreamId = eventStreamId;
            EventNumber = eventNumber;
            EventType = eventType;
            Created = created;
            DomainEvent = domainEvent;
            Metadata = metadata;
        }

        public StreamId EventStreamId { get; }

        public long EventNumber { get; }

        public string EventType { get; }

        public DateTime Created { get; }

        public T DomainEvent { get; }

        public EventMetadata? Metadata { get; }

    }
}