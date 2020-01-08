using System;
using System.Collections.Generic;

namespace Amethyst.EventStore.Postgres.Contexts
{
    public sealed class SinglePartitionContext : IEventStoreContext
    {
        private readonly IEventPublisher _publisher;

        public SinglePartitionContext(IEventPublisher publisher)
            => _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));

        public int GetPartitionsCount(string category) => 1;

        public string GetSchema(StreamId id)
            => id.Category.ToLower();

        public string GetSchema(string category, int partition) => category.ToLower();

        public int GetPartition(StreamId stream) => 0;

        public IReadOnlyCollection<string> GetSchemas(string category)
            => new[] { category.ToLower() };

        public IEventPublisher GetPublisher(StreamId stream) => _publisher;
    }
}