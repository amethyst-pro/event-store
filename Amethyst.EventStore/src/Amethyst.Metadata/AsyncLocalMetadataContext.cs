﻿using System;
using System.Threading;

namespace Amethyst.Metadata
{
    public sealed class AsyncLocalMetadataContext : IMutableMetadataContext
    {
        public static AsyncLocalMetadataContext Instance { get; } = new AsyncLocalMetadataContext();

        private readonly AsyncLocal<EventMetadata?> _localMetadata = new AsyncLocal<EventMetadata?>();
        private readonly AsyncLocal<(long eventNumber, DateTimeOffset createdAt)?> _localEventParams =
            new AsyncLocal<(long eventNumber, DateTimeOffset createdAt)?>();

        private AsyncLocalMetadataContext()
        {
        }

        public EventMetadata? GetMetadata()
            => _localMetadata.Value;

        public long? EventNumber
            => _localEventParams.Value?.eventNumber;

        public DateTimeOffset? CreatedAt 
            => _localEventParams.Value?.createdAt;

        public void SetMetadata(EventMetadata metadata) 
            => _localMetadata.Value = metadata;

        public void SetEventParameters(long eventNumber, DateTimeOffset createdAt) 
            => _localEventParams.Value = (eventNumber, createdAt);
    }
}
