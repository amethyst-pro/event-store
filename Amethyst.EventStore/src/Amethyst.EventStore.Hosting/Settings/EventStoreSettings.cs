using System;
using System.Collections.Generic;
using Amethyst.EventStore.Postgres.Configuration;

namespace Amethyst.EventStore.Hosting.Settings
{
    public sealed class EventStoreSettings 
    {
        public EventStoreSettings(
            StoreSettings store,
            IReadOnlyDictionary<Type, object> aggregateFactories)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            AggregateFactories = aggregateFactories ?? throw new ArgumentNullException(nameof(aggregateFactories));
        }
        
        public EventStoreSettings(
            StoreSettings store,
            IReadOnlyDictionary<Type, object> aggregateFactories,
            SnapshotsSettings snapshots)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            AggregateFactories = aggregateFactories ?? throw new ArgumentNullException(nameof(aggregateFactories));
            Snapshots = snapshots ?? throw new ArgumentNullException(nameof(snapshots));
        }
        
        public StoreSettings Store { get; }
        
        public SnapshotsSettings Snapshots { get; }
        
        public IReadOnlyDictionary<Type, object> AggregateFactories { get; }
    }
}