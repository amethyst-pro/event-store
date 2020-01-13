using Amethyst.EventStore.Abstractions;

namespace Amethyst.EventStore.Postgres.Reading
{
    public sealed class RecordedEventMaterializer : IEventMaterializer<RecordedEvent>
    {
        public RecordedEvent Create(
            in StreamId stream,
            in EventHeader header,
            IEventDataReader reader)
        {
            return new RecordedEvent(
                stream,
                header,
                reader.GetRawData(),
                reader.GetRawMetadata());
        }
    }
}