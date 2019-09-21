using System;

namespace Amethyst.EventStore.Streams.Abstractions.Metadata
{
    public interface IMetadataContext
    {
        EventMetadata? GetMetadata();
        
        long? EventNumber { get; }
        
        DateTimeOffset? Created { get; }
    }
}