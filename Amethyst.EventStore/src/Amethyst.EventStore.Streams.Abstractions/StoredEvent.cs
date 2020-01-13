using System;
using Amethyst.Metadata;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public sealed class StoredEvent
    {
        public readonly StreamId EventStreamId;
        public readonly Guid EventId;
        public readonly long EventNumber;
        public readonly string EventType;
        public readonly DateTime Created;
        public readonly object Event;
        public readonly EventMetadata? Metadata;

        public StoredEvent(
            StreamId eventStreamId,
            Guid eventId,
            long eventNumber,
            string eventType,
            DateTime created,
            object @event,
            EventMetadata? metadata)
        {
            EventStreamId = eventStreamId;
            EventId = eventId;
            EventNumber = eventNumber;
            EventType = eventType;
            Created = created;
            Event = @event;
            Metadata = metadata;
        }
    }
}