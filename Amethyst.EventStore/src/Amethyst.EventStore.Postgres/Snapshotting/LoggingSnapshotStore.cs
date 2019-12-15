using System;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Postgres.Snapshotting
{
    public sealed class LoggingSnapshotStore : ISnapshotStore
    {
        private readonly ISnapshotStore _store;
        private readonly ILogger<LoggingSnapshotStore> _logger;
        
        public LoggingSnapshotStore(ISnapshotStore store, ILogger<LoggingSnapshotStore> logger)
        {
            _store = store;
            _logger = logger;
        }

        public Task<Maybe<IAggregateSnapshot>> ReadSnapshotAsync<T, TId>(StreamId stream) 
            where T : IAggregate<TId>
            => _store.ReadSnapshotAsync<T, TId>(stream);

        public Task SaveSnapshotAsync<T, TId>(IAggregateSnapshot snapshot, StreamId stream)
            where T : IAggregate<TId>
        {
            try
            {
                return _store.SaveSnapshotAsync<T, TId>(snapshot, stream);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Snapshot saving error.");
                throw;
            }
        }
    }
}