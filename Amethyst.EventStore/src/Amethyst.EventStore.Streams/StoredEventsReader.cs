using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Amethyst.EventStore.Streams.Abstractions;
using Amethyst.EventStore.Streams.Abstractions.Metadata;

namespace Amethyst.EventStore.Streams
{
    public sealed class StoredEventsReader : IDbEventReader<StoredEvent>
    {
        private readonly IEventTypeRegistry _typeRegistry;
        private readonly IEventSerializer _serializer;
        private readonly IMetadataSerializer _metadataSerializer;

        public StoredEventsReader(
            IEventTypeRegistry typeRegistry,
            IEventSerializer serializer,
            IMetadataSerializer metadataSerializer)
        {
            _typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
            _serializer = serializer;
            _metadataSerializer = metadataSerializer;
        }

        public async Task<ReadResult<StoredEvent>> Read(StreamId stream, DbDataReader reader, EventFieldMap map)
        {
            var events = new List<StoredEvent>();

            while (await reader.ReadAsync())
            {
                var eventId = reader.GetGuid(map.IdIndex);
                var eventNumber = reader.GetInt64(map.NumberIndex);
                var eventType = _typeRegistry.GetTypeName(
                    reader.GetInt16(map.TypeIndex), stream);
                var created = reader.GetDateTime(map.CreatedIndex);
                var metadata = _metadataSerializer.Deserialize(reader.GetStream(map.MetadataIndex));
                var data = _serializer.Deserialize(eventType, eventNumber, created,
                    reader.GetStream(map.DataIndex), metadata);

                events.Add(new StoredEvent(
                    stream,
                    eventId,
                    eventNumber,
                    eventType,
                    created,
                    0,
                    data,
                    metadata));
            }

            return new ReadResult<StoredEvent>(
                events,
                events.Count > 0 ? events[events.Count - 1].EventNumber : 0L);
        }
    }
}