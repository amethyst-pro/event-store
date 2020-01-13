using System;

namespace Amethyst.Metadata
{
    public interface IMutableMetadataContext : IMetadataContext
    {
        void SetMetadata(EventMetadata metadata);

        void SetEventParameters(long eventNumber, DateTimeOffset createdAt);
    }
}
