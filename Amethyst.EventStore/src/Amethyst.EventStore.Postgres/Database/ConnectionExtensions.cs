using System.Threading.Tasks;
using Npgsql;

namespace Amethyst.EventStore.Postgres.Database
{
    public static class ConnectionExtensions
    {
        public static async Task OpenWithSchemaAsync(this NpgsqlConnection connection, string defaultSchema)
        {
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SET SCHEMA '" + defaultSchema + "'";
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
