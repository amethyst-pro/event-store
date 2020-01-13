using System;

namespace Amethyst.Metadata
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
