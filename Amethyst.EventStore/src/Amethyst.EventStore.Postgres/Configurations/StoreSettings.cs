using System;
using System.Collections.Generic;
using System.Linq;
using Amethyst.EventStore.Postgres.Database;

namespace Amethyst.EventStore.Postgres.Configurations
{
    public sealed class StoreSettings
    {
        public StoreSettings(
            PgsqlConnections connections,
            IReadOnlyDictionary<string, IEventStoreContext> contexts)
        {
            Connections = connections ?? throw new ArgumentNullException(nameof(connections));
            
            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));
            
            if (!contexts.Any())
                throw new ArgumentException("Contexts not specified.", nameof(contexts));

            Contexts = contexts;
        }
        
        public TimeSpan OutboxInterval { get; } = TimeSpan.FromMinutes(3);

        public PgsqlConnections Connections { get; }

        public IReadOnlyDictionary<string, IEventStoreContext> Contexts { get; }
    }
}