using System;
using Npgsql;

namespace Amethyst.EventStore.Postgres.Database
{
    public sealed class SchemaInstaller : IDisposable
    {
        private NpgsqlConnection _connection;

        public SchemaInstaller(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void Install(string schema)
        {
            using (var command = new NpgsqlCommand($@"
                CREATE SCHEMA IF NOT EXISTS ""{schema}"";

                CREATE TABLE IF NOT EXISTS ""{schema}"".streams (
                    id                uuid    PRIMARY KEY
                   ,last_event_number bigint  NOT NULL
                   ,last_sent_event_number bigint NOT NULL DEFAULT -1
                   ,truncate_before bigint NOT NULL DEFAULT -1 
                );

                CREATE TABLE IF NOT EXISTS ""{schema}"".event_types (
                    id    smallserial       PRIMARY KEY
                   ,name  varchar(512)     CONSTRAINT uq_event_types_name UNIQUE NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ""{schema}"".events (
                    id           bigserial    PRIMARY KEY         
                   ,event_id     uuid         UNIQUE NOT NULL
                   ,stream_id    uuid         NOT NULL
                   ,number       bigint       NOT NULL
                   ,type_id      smallint     NOT NULL
                   ,data         bytea        NOT NULL
                   ,metadata     bytea        NOT NULL                      
                   ,created      timestamptz  NOT NULL DEFAULT now()
                   ,CONSTRAINT uq_events_stream_number UNIQUE (stream_id, number)
                   ,CONSTRAINT fk_events_streams FOREIGN KEY (stream_id) REFERENCES ""{schema}"".streams (id)
                   ,CONSTRAINT fk_events_event_types FOREIGN KEY (type_id) REFERENCES ""{schema}"".event_types (id)
                );

                CREATE TABLE IF NOT EXISTS ""{schema}"".outbox (
                    stream_id    uuid    PRIMARY KEY
                );

                 CREATE TABLE IF NOT EXISTS ""{schema}"".snapshots (      
                   stream_id    uuid         PRIMARY KEY
                   ,data         bytea        NOT NULL
                   ,version      bigint       NOT NULL
                   ,created      timestamptz  NOT NULL DEFAULT now()
                );",
                _connection))
            { 
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
        }
    }
}