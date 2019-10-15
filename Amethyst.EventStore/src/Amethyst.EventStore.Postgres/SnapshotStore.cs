using System;
using System.Threading.Tasks;
using Npgsql;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Postgres
{
    public sealed class SnapshotStore : ISnapshotStore
    {
        private readonly PgsqlConnections _connections;
        private readonly IEventStoreContext _context;
        private readonly ISnapshotSerializer _serializer;

        public SnapshotStore(
            PgsqlConnections connections, 
            IEventStoreContext context,
            ISnapshotSerializer serializer)
        {
            _connections = connections;
            _context = context;
            _serializer = serializer;
        }
        
        public async Task<Maybe<IAggregateSnapshot>> ReadSnapshotAsync<T, TId>(StreamId stream) where T : IAggregate<TId>
        {
            using (var connection = new NpgsqlConnection(_connections.ReadOnly))
            {
                await connection.OpenWithSchemaAsync(_context.GetSchema(stream));
                
                var streamIdParameter = new NpgsqlParameter<Guid>("streamId", stream.Id);

                const string query = @"
                    SELECT data 
                    FROM snapshots
                    WHERE stream_id = @streamId";

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.Add(streamIdParameter);
                    await cmd.PrepareAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.Read())
                            return default;
                
                        var snapshot = _serializer.Deserialize<T, TId>(reader.GetStream(0));
                
                        return snapshot == null 
                            ? new Maybe<IAggregateSnapshot>() 
                            : new Maybe<IAggregateSnapshot>(snapshot);
                    }
                }
            }
        }

        public async Task SaveSnapshotAsync<T, TId>(IAggregateSnapshot snapshot, StreamId stream) 
            where T : IAggregate<TId>
        {
            if (snapshot == null) 
                throw new ArgumentNullException(nameof(snapshot));
            
            using (var connection = new NpgsqlConnection(_connections.Default))
            {
                await connection.OpenWithSchemaAsync(_context.GetSchema(stream));
                
                var streamIdParameter = new NpgsqlParameter<Guid>("streamId", stream.Id);
                var dataParameter = new NpgsqlParameter<byte[]>("data", _serializer.Serialize<T, TId>(snapshot));
                var versionParameter = new NpgsqlParameter<long>("version", snapshot.Version);

                const string sql = 
                    @"INSERT INTO snapshots (stream_id, data, version)
                     VALUES (@streamId, @data, @version)
                     ON CONFLICT (stream_id) DO UPDATE SET data = excluded.data, version = excluded.version";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddRange(
                        new NpgsqlParameter[]
                        {
                            streamIdParameter,
                            dataParameter,
                            versionParameter
                        });

                    await cmd.PrepareAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}