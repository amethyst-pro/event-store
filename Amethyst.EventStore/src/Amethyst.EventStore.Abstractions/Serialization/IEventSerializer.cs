using System;
using Amethyst.EventStore.Abstractions.Metadata;

namespace Amethyst.EventStore.Abstractions.Serialization
{
    public interface IEventSerializer
    {
        object Deserialize(RecordedEvent @event);

        EventData Serialize(object @event, Guid eventId, EventMetadata? metadata = default);

        bool HasKnownType(RecordedEvent @event);
    }
}