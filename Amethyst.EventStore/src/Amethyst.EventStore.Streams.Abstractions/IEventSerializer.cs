using System;
using System.IO;
using Amethyst.EventStore.Streams.Abstractions.Metadata;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public interface IEventSerializer
    {
        bool HasKnownType(string eventType);

        EventData Serialize(object @event, Guid eventId, EventMetadata? metadata = default);

        object Deserialize(
            string eventType,
            long eventNumber,
            DateTime eventCreated,
            Stream data,
            EventMetadata? metadata);
    }
}