using System;
using Amethyst.EventStore.Streams.Abstractions.Metadata;

namespace Amethyst.EventStore.Streams.Meta
{
    public sealed class NullMetadataContext : IMetadataContext
    {
        public static readonly IMetadataContext Instance = new NullMetadataContext();

        private NullMetadataContext()
        {
        }

        public EventMetadata? GetMetadata()
        {
            return default;
        }

        public long? EventNumber => null;

        public DateTimeOffset? CreatedAt => null;
    }
}
