using System;
using System.Collections.Generic;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Abstractions.Storage;

namespace Amethyst.EventStore.Postgres.Context
{
    public sealed class SinglePartitionContext : IEventStoreContext
    {
        private readonly IEventPublisher _publisher;

        public SinglePartitionContext(IEventPublisher publisher)
            => _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));

        public string GetSchema(StreamId id)
            => id.Category.ToLower();

        public IReadOnlyCollection<string> GetSchemas(string category)
            => new[] {category.ToLower()};

        public IEventPublisher GetPublisher(StreamId stream) => _publisher;
    }
}