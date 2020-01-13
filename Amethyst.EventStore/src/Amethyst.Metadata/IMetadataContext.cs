using System;

namespace Amethyst.Metadata
{
    public interface IMetadataContext
    {
        EventMetadata? GetMetadata();
        
        long? EventNumber { get; }
        DateTimeOffset? CreatedAt { get; }
    }
}
