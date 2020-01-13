using System;

namespace Amethyst.EventStore.Abstractions
{
    public sealed class RecordedEvent
    {
        public readonly StreamId StreamId;
        public readonly EventHeader Header;
        public readonly byte[] Data;
        public readonly byte[] Metadata;

        public RecordedEvent(
            StreamId streamId,
            EventHeader header,
            byte[] data,
            byte[] metadata)
        {
            StreamId = streamId;
            Header = header;
            Data = data;
            Metadata = metadata;
        }
    }
}