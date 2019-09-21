using System;
using Amethyst.EventStore.Streams.Abstractions.Metadata;

namespace Amethyst.EventStore.Streams.Abstractions.Serialization
{
    public interface IEventSerializer
    {
        object Deserialize(RecordedEvent @event);

        EventData Serialize(object @event, Guid eventId, EventMetadata? metadata = default);

        bool HasKnownType(RecordedEvent @event);
    }
}