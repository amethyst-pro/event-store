using System.Collections.Generic;

namespace Amethyst.EventStore.Postgres.Contexts
{
    public sealed class CompositeEventStoreContext : IEventStoreContext
    {
        private readonly IReadOnlyDictionary<string, IEventStoreContext> _resolverMaps;

        public CompositeEventStoreContext(IReadOnlyDictionary<string, IEventStoreContext> resolverMaps)
        {
            _resolverMaps = resolverMaps;
        }

        public int GetPartitionsCount(string category) =>
            _resolverMaps[category].GetPartitionsCount(category);

        public string GetSchema(StreamId stream) =>
            _resolverMaps[stream.Category].GetSchema(stream);

        public string GetSchema(string category, int partition) =>
            _resolverMaps[category].GetSchema(category, partition);

        public int GetPartition(StreamId stream) =>
            _resolverMaps[stream.Category].GetPartition(stream);

        public IReadOnlyCollection<string> GetSchemas(string category) =>
            _resolverMaps[category].GetSchemas(category);

        public IEventPublisher GetPublisher(StreamId stream) =>
            _resolverMaps[stream.Category].GetPublisher(stream);
    }
}