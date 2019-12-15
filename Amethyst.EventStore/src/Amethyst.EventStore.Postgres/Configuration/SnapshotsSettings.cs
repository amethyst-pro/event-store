using System;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Abstractions.Storage;

namespace Amethyst.EventStore.Postgres.Configuration
{
    public sealed class SnapshotsSettings
    {
        public SnapshotsSettings(
            ITriggerThresholdResolver triggerResolver, 
            ISnapshotSerializer serializer)
        {
            TriggerResolver = triggerResolver ?? throw new ArgumentNullException(nameof(triggerResolver));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        public ITriggerThresholdResolver TriggerResolver { get; }

        public ISnapshotSerializer Serializer { get; }
    }
}