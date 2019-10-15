using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Reading;

namespace Amethyst.EventStore.Postgres.Reading
{
    public sealed class RecordedEventMaterializer : IEventMaterializer<RecordedEvent>
    {
        public RecordedEvent Create(EventHeader header, IEventDataReader reader)
        {
            return new RecordedEvent(
                header.StreamId,
                header.Id,
                header.Number,
                header.Type,
                header.Created,
                0,
                reader.GetRawData(),
                reader.GetRawMetadata());
        }
    }
}