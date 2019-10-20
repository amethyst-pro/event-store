using System;
using System.Threading;
using Npgsql;

namespace Amethyst.EventStore.Postgres.Database
{
    public sealed class ConnectionFactory : IConnectionFactory
    {
        private readonly PgsqlConnections _connections;
        private readonly int _readOnlyConnectionsPerDefault;
        private static int _connectionCounter;

        public ConnectionFactory(PgsqlConnections connections, int readOnlyConnectionsPerDefault = 3)
        {
            _connections = connections ?? throw new ArgumentNullException(nameof(connections));
            _readOnlyConnectionsPerDefault = readOnlyConnectionsPerDefault;
        }

        public NpgsqlConnection CreateReadConnection(bool allowAsyncReplica = false)
        {
            if (allowAsyncReplica)
                return new NpgsqlConnection(_connections.ReadOnlyAsync);

            var connection = Interlocked.Increment(ref _connectionCounter) % _readOnlyConnectionsPerDefault == 0
                ? _connections.Default
                : _connections.ReadOnly;

            return new NpgsqlConnection(connection);
        }

        public NpgsqlConnection CreateWriteConnection() =>
            new NpgsqlConnection(_connections.Default);
    }
}