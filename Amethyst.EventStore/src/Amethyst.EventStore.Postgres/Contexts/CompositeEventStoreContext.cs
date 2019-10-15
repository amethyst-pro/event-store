using System.Collections.Generic;
using Amethyst.EventStore.Postgres.Publishing;

namespace Amethyst.EventStore.Postgres.Contexts
{
    public sealed class CompositeEventStoreContext : IEventStoreContext
    {
        private readonly IReadOnlyDictionary<string, IEventStoreContext> _resolverMaps;

        public CompositeEventStoreContext(IReadOnlyDictionary<string, IEventStoreContext> resolverMaps)
        {
            _resolverMaps = resolverMaps;
        }

        public string GetSchema(StreamId stream) => 
            _resolverMaps[stream.Category].GetSchema(stream);

        public IReadOnlyCollection<string> GetSchemas(string category) =>
            _resolverMaps[category].GetSchemas(category);

        public IEventPublisher GetPublisher(StreamId stream) =>
            _resolverMaps[stream.Category].GetPublisher(stream);
    }
}