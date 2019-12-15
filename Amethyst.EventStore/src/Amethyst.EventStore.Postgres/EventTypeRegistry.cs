using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Storage;
using Npgsql;

namespace Amethyst.EventStore.Postgres
{
    public sealed class EventTypeRegistry : IEventTypeRegistry
    {
        private readonly DbConnections _connections;
        private readonly IEventStoreContext _context;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<short, string>> _items =
            new ConcurrentDictionary<string, ConcurrentDictionary<short, string>>();

        public EventTypeRegistry(DbConnections connections, IEventStoreContext context)
        {
            _connections = connections;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Load(NpgsqlConnection connection, IReadOnlyCollection<string> schemas)
        {
            foreach (var schema in schemas)
            {
                _items[schema] = GetTypes(schema, connection);
            }
        }

        public short GetOrAddTypeId(string typeName, StreamId streamId)
        {
            var schema = _context.GetSchema(streamId);
            var registry = _items.GetOrAdd(schema, _ => new ConcurrentDictionary<short, string>());
            
            foreach (var pair in registry)
            {
                if (pair.Value.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    return pair.Key;
            }

            lock (registry)
            {
                foreach (var pair in registry)
                {
                    if (pair.Value.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                        return pair.Key;
                }

                var typeId = AddNewType(typeName, schema);
                registry.TryAdd(typeId, typeName);

                return typeId;
            }
        }

        public string GetTypeName(short id, StreamId streamId)
        {
            var schema = _context.GetSchema(streamId);
            var registry = _items.GetOrAdd(schema, GetTypes);

            if (registry.TryGetValue(id, out var type)) 
                return type;
            
            var reloadedTypes = GetTypes(schema);
            _items[schema] = reloadedTypes;

            return reloadedTypes[id];

        }
        
        private ConcurrentDictionary<short, string> GetTypes(string schema)
        {
            using var connection = new NpgsqlConnection(_connections.ReadOnly);
            connection.Open();
            return GetTypes(schema, connection);
        }

        private static ConcurrentDictionary<short, string> GetTypes(string schema, NpgsqlConnection connection)
        {
            var types = new ConcurrentDictionary<short, string>();

            using (var command = new NpgsqlCommand($"SELECT id, name from \"{schema}\".event_types", connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    types.TryAdd(reader.GetInt16(0), reader.GetString(1));
                }
            }

            return types;
        }

        private short AddNewType(string typeName, string schema)
        {
            using var connection = new NpgsqlConnection(_connections.Default);
            connection.Open();

            using var command = new NpgsqlCommand($@"
                    INSERT INTO ""{schema}"".event_types (name)
                    VALUES (@name)
                    ON CONFLICT (name) DO NOTHING;
                    SELECT id FROM ""{schema}"".event_types WHERE name = @name;
                ", connection);
            command.Parameters.AddWithValue("name", typeName);

            var newId = command.ExecuteScalar();
            if (newId == null)
                throw new InvalidOperationException("Can't add new event type");

            return (short) newId;
        }
    }
}