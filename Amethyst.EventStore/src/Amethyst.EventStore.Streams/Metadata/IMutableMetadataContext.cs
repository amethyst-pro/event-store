using System;
using Amethyst.EventStore.Abstractions.Metadata;

namespace Amethyst.EventStore.Streams.Metadata
{
    public interface IMutableMetadataContext : IMetadataContext
    {
        void SetMetadata(EventMetadata metadata);

        void SetEventParameters(long eventNumber, DateTimeOffset createdAt);
    }
}