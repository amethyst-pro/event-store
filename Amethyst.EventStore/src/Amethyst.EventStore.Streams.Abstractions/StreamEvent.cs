
using Amethyst.Metadata;

namespace Amethyst.EventStore.Streams.Abstractions
{
    public readonly struct StreamEvent
    {
        public readonly object Event;
        public readonly EventMetadata? Metadata;

        public StreamEvent(object @event, EventMetadata? metadata = default)
        {
            Event = @event;
            Metadata = metadata;
        }
    }
}