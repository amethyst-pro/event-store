using System.Collections.Generic;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Abstractions.Storage;

namespace Amethyst.EventStore.Postgres.Context
{
    public sealed class CompositeEventStoreContext : IEventStoreContext
    {
        private readonly IReadOnlyDictionary<string, IEventStoreContext> _contexts;

        public CompositeEventStoreContext(IReadOnlyDictionary<string, IEventStoreContext> contexts)
            =>  _contexts = contexts;

        public string GetSchema(StreamId stream) => 
            _contexts[stream.Category].GetSchema(stream);

        public IReadOnlyCollection<string> GetSchemas(string category) =>
            _contexts[category].GetSchemas(category);

        public IEventPublisher GetPublisher(StreamId stream) =>
            _contexts[stream.Category].GetPublisher(stream);
    }
}