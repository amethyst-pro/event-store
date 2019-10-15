using System;
using Amethyst.EventStore.Streams.Abstractions.Metadata;

namespace Amethyst.EventStore.Streams.Meta
{
    public interface IMutableMetadataContext : IMetadataContext
    {
        void SetMetadata(EventMetadata metadata);

        void SetEventParameters(long eventNumber, DateTimeOffset createdAt);
    }
}
