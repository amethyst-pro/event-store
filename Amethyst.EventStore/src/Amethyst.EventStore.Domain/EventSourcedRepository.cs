using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.Domain;
using Amethyst.EventStore.Streams.Abstractions;
using Amethyst.EventStore.Streams.Abstractions.Metadata;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Domain
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
            _metadataContext = metadataContext ?? NullMetadataContext.Instance;
        }

        public async Task<Maybe<TAggregate>> GetAsync(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var stream = GetStream(id);
            var readResult = await stream.ReadEventsForward();

            return readResult.Events.Count > 0
                ? _aggregateFactory.Create(id, readResult.NextExpectedVersion, readResult.Events.Select(x => (IDomainEvent)x.Event).ToArray())
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
            var streamId = _streamIdResolver.Resolve<TAggregate, TId>(id);
            return new Stream(streamId, _reader, _writer, _metadataContext);
        }
    }
}