using System;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Metadata;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Domain;
using Amethyst.EventStore.Domain.Abstractions;
using Amethyst.EventStore.Streams.Metadata;
using Amethyst.EventStore.Streams.Resolvers;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Streams.Repositories
{
    public sealed class EventSourcedRepository<TAggregate, TId> : IRepository<TAggregate, TId>
        where TAggregate : IAggregate<TId>
        where TId : IGuidId
    {
        private readonly IAggregateStreamIdResolver _streamIdResolver;
        private readonly IAggregateFactory<TAggregate, TId> _aggregateFactory;
        private readonly IEventSerializer _serializer;
        private readonly IMetadataContext _metadataContext;
        private readonly IEventStore _store;

        public EventSourcedRepository(
            IAggregateStreamIdResolver streamIdResolver,
            IAggregateFactory<TAggregate, TId> aggregateFactory,
            IEventStore eventStore,
            IEventSerializer serializer,
            IMetadataContext metadataContext = null)
        {
            _streamIdResolver = streamIdResolver ?? throw new ArgumentNullException(nameof(streamIdResolver));
            _aggregateFactory = aggregateFactory ?? throw new ArgumentNullException(nameof(aggregateFactory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _metadataContext = metadataContext ?? NullMetadataContext.Instance;
            _store = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        public async Task<Maybe<TAggregate>> GetAsync(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var stream = GetStream(id);
            var readResult = await stream.ReadEventsForward();

            return readResult.Events.Count > 0
                ? _aggregateFactory.Create(id, readResult.NextExpectedVersion,
                    readResult.Events.Select(x => x.Data).ToArray())
                : new Maybe<TAggregate>();
        }

        public async Task SaveAsync(TAggregate aggregate)
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));

            if (!aggregate.UncommittedEvents.Any())
                return;

            var stream = GetStream(aggregate.Id);
            var result = await stream.Append(aggregate.UncommittedEvents, aggregate.Version);

            aggregate.ClearUncommittedEvents(result.NextExpectedVersion);
        }

        public Task<bool> ExistsAsync(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var stream = GetStream(id);
            return stream.HasEvents();
        }

        private Stream GetStream(TId id)
        {
            var streamName = _streamIdResolver.Resolve<TAggregate, TId>(id);
            return new Stream(_store, _serializer, streamName, _metadataContext);
        }
    }
}