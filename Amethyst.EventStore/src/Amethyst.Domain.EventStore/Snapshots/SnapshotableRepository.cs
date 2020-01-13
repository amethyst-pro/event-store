using System;
using System.Linq;
using System.Threading.Tasks;
using Amethyst.EventStore;
using Amethyst.EventStore.Streams.Abstractions;
using Amethyst.Metadata;
using SharpJuice.Essentials;

namespace Amethyst.Domain.EventStore.Snapshots
{
    public interface IAggregateOptions<TAggregate>
    {
        public long SnapshotThreshold { get; }
    }

    public sealed class SnapshotableRepository<TAggregate, TId, TSnapshot> : IRepository<TAggregate, TId>
        where TAggregate : ISnapshotableAggregate<TId, TSnapshot>
        where TId : IGuidId
        where TSnapshot : IAggregateSnapshot
    {
        private readonly IAggregateOptions<TAggregate> _options;
        private readonly IStreamReader _reader;
        private readonly IStreamWriter _writer;
        private readonly IAggregateStreamIdResolver _streamIdResolver;
        private readonly ISnapshotableAggregateFactory<TAggregate, TId, TSnapshot> _factory;
        private readonly IMetadataContext _metadataContext;

        public SnapshotableRepository(
            IAggregateOptions<TAggregate> options,
            IAggregateStreamIdResolver streamIdResolver,
            ISnapshotableAggregateFactory<TAggregate, TId, TSnapshot> factory,
            IStreamReader reader,
            IStreamWriter writer,
            IMetadataContext metadataContext = null)
        {
            _options = options;
            _reader = reader;
            _writer = writer;
            _streamIdResolver = streamIdResolver;
            _factory = factory;
            _metadataContext = metadataContext;
        }

        public async Task<Maybe<TAggregate>> Get(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var readResult = await _reader.ReadStream<TSnapshot>(GetStreamId(id));

            if (readResult.Snapshot.Any())
            {
                return _factory.Create(id, readResult.LastEventNumber, readResult.Snapshot.Single(),
                    readResult.HeadEvents.Select(x => x.Event).ToArray());
            }

            return readResult.HeadEvents.Length > 0
                ? _factory.Create(
                    id,
                    readResult.LastEventNumber,
                    readResult.HeadEvents.Select(x => x.Event).ToArray())
                : new Maybe<TAggregate>();
        }

        public async Task Save(TAggregate aggregate)
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));

            if (!aggregate.UncommittedEvents.Any())
                return;

            var metadata = _metadataContext?.GetMetadata();
            var stream = GetStreamId(aggregate.Id);

            var result = await _writer.Append(
                stream,
                aggregate.Version.OrElse(ExpectedVersion.Any),
                aggregate.UncommittedEvents.Select(e => new StreamEvent(e, metadata)));

            aggregate.ClearUncommittedEvents(result.NextExpectedVersion);

            if (result.NextExpectedVersion - aggregate.StoredSnapshotVersion.OrElse(-1) < _options.SnapshotThreshold)
                return;

            var _ = _writer.SaveSnapshot(stream, aggregate.GetSnapshot());
        }

        public Task<bool> Exists(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return _reader.Exists(GetStreamId(id));
        }

        private StreamId GetStreamId(TId id) =>
            _streamIdResolver.Resolve<TAggregate, TId>(id);
    }
}