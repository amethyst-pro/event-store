using System;
using System.Collections.Generic;
using System.Linq;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Abstractions.Storage;

namespace Amethyst.EventStore.Postgres.Configuration
{
    public sealed class StoreSettings
    {
        public StoreSettings(
            DbConnections connections,
            IReadOnlyDictionary<string, IEventStoreContext> contexts, 
            IEventSerializer serializer)
        {
            Connections = connections ?? throw new ArgumentNullException(nameof(connections));
            
            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));
            
            if (!contexts.Any())
                throw new ArgumentException("Contexts not specified.", nameof(contexts));

            Contexts = contexts;
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        public TimeSpan OutboxInterval { get; } = TimeSpan.FromMinutes(3);

        public DbConnections Connections { get; }

        public IReadOnlyDictionary<string, IEventStoreContext> Contexts { get; }
        
        public IEventSerializer Serializer { get; }
    }
}