using System;
using System.Collections.Generic;
using System.Linq;
using Amethyst.EventStore.Postgres.Publishing;

namespace Amethyst.EventStore.Postgres.Contexts
{
    public sealed class MultiPartitionContext : IEventStoreContext
    {
        private readonly int _partitionCount;
        private readonly IEventPublisher _publisher;

        public MultiPartitionContext(int partitionCount, IEventPublisher publisher)
        {
            if (partitionCount < 1)
                throw new ArgumentOutOfRangeException(nameof(partitionCount), partitionCount, "Must be greater than 0");

            _partitionCount = partitionCount;
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public string GetSchema(StreamId id)
        {
            Span<byte> idBytes = stackalloc byte[16];
            
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            if (!id.Id.TryWriteBytes(idBytes))
                throw new InvalidOperationException("Can't write guid to bytes");

            var hash = BitConverter.ToInt32(idBytes.Slice(0, 4))
                       ^ BitConverter.ToInt32(idBytes.Slice(4, 4))
                       ^ BitConverter.ToInt32(idBytes.Slice(8, 4))
                       ^ BitConverter.ToInt32(idBytes.Slice(12, 4));

            var partition = Math.Abs(hash % _partitionCount);

            return id.Category.ToLower() + "_" + partition;
        }

        public IReadOnlyCollection<string> GetSchemas(string category)
        {
            var schemaPrefix = category.ToLower() + "_";

            return Enumerable.Range(0, _partitionCount)
                .Select(i => schemaPrefix + i)
                .ToArray();
        }

        public IEventPublisher GetPublisher(StreamId stream) => _publisher;
    }
}