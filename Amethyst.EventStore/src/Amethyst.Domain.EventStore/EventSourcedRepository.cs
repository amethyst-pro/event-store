using System;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore;
using Amethyst.EventStore.Streams.Abstractions;
using Amethyst.EventStore.Streams.Abstractions.Metadata;
using SharpJuice.Essentials;

namespace Amethyst.Domain.EventStore
{
    public sealed class EventSourcedRepository<TAggregate, TId> : IRepository<TAggregate, TId>
        where TAggregate : IAggregate<TId>
        where TId : IGuidId
    {
        private readonly IAggregateStreamIdResolver _streamIdResolver;
        private readonly IAggregateFactory<TAggregate, TId> _aggregateFactory;
        private readonly IMetadataContext _metadataContext;
        private readonly IStreamReader _reader;
        private readonly IStreamWriter _writer;

        public EventSourcedRepository(
            IAggregateStreamIdResolver streamIdResolver,
            IAggregateFactory<TAggregate, TId> aggregateFactory,
            IStreamReader reader,
            IStreamWriter writer,
            IMetadataContext metadataContext = null)
        {
            _streamIdResolver = streamIdResolver ?? throw new ArgumentNullException(nameof(streamIdResolver));
            _aggregateFactory = aggregateFactory ?? throw new ArgumentNullException(nameof(aggregateFactory));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _metadataContext = metadataContext;
        }

        public async Task<Maybe<TAggregate>> GetAsync(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

           var readResult = await _reader.ReadEventsForward(GetStreamId(id));

            return readResult.Events.Count > 0
                ? _aggregateFactory.Create(
                    id, 
                    readResult.LastEventNumber, 
                    readResult.Events.Select(x => x.Event).ToArray())
                : new Maybe<TAggregate>();
        }

        public async Task SaveAsync(TAggregate aggregate)
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));

            if (!aggregate.UncommittedEvents.Any())
                return;
            
            var metadata = _metadataContext?.GetMetadata();

            var result = await _writer.Append(
                GetStreamId(aggregate.Id),
                aggregate.Version.OrElse(ExpectedVersion.Any),
                aggregate.UncommittedEvents.Select(e => new StreamEvent(e, metadata)));

            aggregate.ClearUncommittedEvents(result.NextExpectedVersion);
        }

        public Task<bool> ExistsAsync(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return _reader.HasEvents(GetStreamId(id));
        }

        private StreamId GetStreamId(TId id) =>
            _streamIdResolver.Resolve<TAggregate, TId>(id);
    }
}