using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Metadata;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Domain.Abstractions;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Streams
{
    public sealed class Stream : IStream
    {
        private readonly IEventStore _store;
        private readonly IEventSerializer _serializer;
        private readonly IMetadataContext _metadataContext;

        public Stream(
            IEventStore store, 
            IEventSerializer serializer, 
            StreamId id,
            IMetadataContext metadataContext)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            Id = id;
            _metadataContext = metadataContext;
        }

        public StreamId Id { get; }

        public async Task<long> GetLastEventNumber()
        {
            return (await ReadLastEvent())
                .Bind(e => e.Number)
                .OrElse(-1);
        }

        public async Task<StreamReadResult> ReadEventsForward(Maybe<long> startPosition = default)
        {
            var recordedEvents = await _store.ReadStreamEventsForwardAsync(
                Id,
                startPosition.OrElse(StreamPosition.Start));

            if (recordedEvents.Status != ReadStatus.Success)
                return new StreamReadResult(Array.Empty<StoredEvent>(), ExpectedVersion.NoStream);

            var events = recordedEvents.Events
                .AsParallel()
                .AsOrdered()
                .Select(ToStoredEvent)
                .ToArray();

            return new StreamReadResult(events, recordedEvents.LastEventNumber);
        }

        public Task<Maybe<StoredEvent>> ReadFirstEvent()
            => ReadEvent(StreamPosition.Start);

        public Task<Maybe<StoredEvent>> ReadLastEvent() 
            => ReadEvent(StreamPosition.End);

        public Task<WriteResult> Append(IEnumerable<IDomainEvent> events, Maybe<long> expectedVersion = default)
        {
            return _store.AppendToStreamAsync(
                Id,
                expectedVersion.OrElse(ExpectedVersion.NoStream),
                events.Select(e => _serializer.Serialize(e, Guid.NewGuid(), _metadataContext.GetMetadata())));
        }

        public async Task<bool> HasEvents()
        {
            var result = await _store.ReadEventAsync(Id, StreamPosition.Start);
            return result.Status == ReadStatus.Success && result.Event != null;
        }

        private async Task<Maybe<StoredEvent>> ReadEvent(long streamPosition)
        {
            var result = await _store.ReadEventAsync(Id, streamPosition);
            return GetEventFrom(result);
        }

        private Maybe<StoredEvent> GetEventFrom(EventReadResult result)
        {
            switch (result.Status)
            {
                case ReadStatus.Success:
                    return ToStoredEvent(result.Event);
                case ReadStatus.NotFound:
                case ReadStatus.NoStream:
                    return new Maybe<StoredEvent>();
                case ReadStatus.StreamDeleted:
                    throw new InvalidOperationException($"Stream {Id} has been deleted.");
                default:
                    throw new NotSupportedException(result.Status.ToString());
            }
        }

        private StoredEvent ToStoredEvent(RecordedEvent recordedEvent)
            => new StoredEvent(
                recordedEvent.StreamId,
                recordedEvent.Id,
                recordedEvent.Number,
                recordedEvent.Type,
                recordedEvent.Created,
                (IDomainEvent)_serializer.Deserialize(recordedEvent));
    }
}