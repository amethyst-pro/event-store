using Npgsql;

namespace Amethyst.EventStore.Postgres
{
    public interface IConnectionFactory
    {
        NpgsqlConnection CreateReadConnection(bool allowAsyncReplica = false);
        NpgsqlConnection CreateWriteConnection();
    }
}