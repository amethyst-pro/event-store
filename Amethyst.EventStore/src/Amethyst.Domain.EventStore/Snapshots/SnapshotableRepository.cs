using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Amethyst.Domain.EventStore.Snapshots
{
    public sealed class SnapshotableRepository<TAggregate, TId> : IRepository<TAggregate, TId>
        where TAggregate : ISnapshotableAggregate<TId>
        where TId : IGuidId
    {
        private readonly IAggregateStreamIdResolver _streamIdResolver;
        private readonly IAggregateFactory<TAggregate, TId> _aggregateFactory;
        private readonly IEventSerializer _serializer;
        private readonly ITriggerThresholdResolver _thresholdResolver;
        private readonly IEventStore _eventStore;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IMetadataContext _metadataContext;

        public SnapshotableRepository(
            IAggregateStreamIdResolver streamIdResolver,
            ITriggerThresholdResolver thresholdResolver,
            IAggregateFactory<TAggregate, TId> aggregateFactory,
            IEventStore eventStore,
            IEventSerializer serializer,
            ISnapshotStore snapshotStore,
            IMetadataContext metadataContext = null)
        {
            _streamIdResolver = streamIdResolver ?? throw new ArgumentNullException(nameof(streamIdResolver));
            _thresholdResolver = thresholdResolver ?? throw new ArgumentNullException(nameof(thresholdResolver));
            _aggregateFactory = aggregateFactory ?? throw new ArgumentNullException(nameof(aggregateFactory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _metadataContext = metadataContext ?? NullMetadataContext.Instance;
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _snapshotStore = snapshotStore ?? throw new ArgumentException(nameof(snapshotStore));
        }
        
        public async Task<Maybe<TAggregate>> GetAsync(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            
            var stream = GetStream(id);
            
            var snapshot = await _snapshotStore.ReadSnapshotAsync<TAggregate, TId>(stream.Id);
            if (!snapshot.Any())
                return await GetByEventsAsync(id, stream);

            return await GetBySnapsotAsync(id, stream, snapshot.Single());
        }

        public async Task SaveAsync(TAggregate aggregate)
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));

            if (!aggregate.UncommittedEvents.Any())
                return;

            var stream = GetStream(aggregate.Id);
            var result = await stream.Append(aggregate.UncommittedEvents, aggregate.Version);

            aggregate.ClearUncommittedEvents(result.NextExpectedVersion);
            
            var threshold = _thresholdResolver.ResolveByAggregate<TAggregate>();
            if (result.NextExpectedVersion - aggregate.StoredSnapshotVersion < threshold)
                return;
       
            var _ = _snapshotStore.SaveSnapshotAsync<TAggregate, TId>(aggregate.GetSnapshot(), stream.Id);
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
            
            return new Stream(_eventStore, _serializer, streamId, _metadataContext);
        }
        
        private async Task<Maybe<TAggregate>> GetByEventsAsync(TId id, IStream stream)
        {
            var readEventsResult = await stream.ReadEventsForward();
            return readEventsResult.Events.Count > 0
                ? _aggregateFactory.Create(id, readEventsResult.NextExpectedVersion, readEventsResult.Events.Select(x => x.Data).ToArray())
                : new Maybe<TAggregate>();
        }
        
        private async Task<Maybe<TAggregate>> GetBySnapsotAsync(TId id, IStream stream, IAggregateSnapshot snapshot)
        {
            var readResult = await stream.ReadEventsForward(snapshot.Version + 1);

            var version = readResult.Events.Any()
                ? readResult.NextExpectedVersion
                : snapshot.Version;
            
            return _aggregateFactory.Create(id, version, snapshot, readResult.Events.Select(x => x.Data).ToArray());
        }
    }
}