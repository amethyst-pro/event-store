using System;
using System.Collections.Generic;
using System.Linq;
using Amethyst.EventStore.Abstractions;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Abstractions.Storage;
using Amethyst.EventStore.Hosting.Settings;
using Amethyst.EventStore.Postgres;
using Amethyst.EventStore.Postgres.Configuration;
using Amethyst.EventStore.Postgres.Context;
using Amethyst.EventStore.Postgres.Publishing;
using Amethyst.EventStore.Postgres.Snapshotting;
using Amethyst.EventStore.Streams.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Amethyst.EventStore.Hosting
{
    public static class EventStoreExtensions
    {
        public static IServiceCollection AddEventStore(
            this IServiceCollection services,
            EventStoreSettings settings,
            ILoggerFactory loggerFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            if (settings.Store == null)
                throw new ArgumentException("Store settings not specified.", nameof(settings.Store));
            
            if (settings.AggregateFactories == null)
                throw new ArgumentException("Factories not specified.", nameof(settings.AggregateFactories));
            
            var compositeContext = new CompositeEventStoreContext(settings.Store.Contexts);
            var typesRegistry = new EventTypeRegistry(settings.Store.Connections, compositeContext);

            InitializeSchemas(settings.Store, typesRegistry);
            
            var reader = new EventsReader(settings.Store.Connections, compositeContext, typesRegistry);

            var eventStore = new Postgres.EventStore(
                reader,
                new EventsWriter(settings.Store.Connections, compositeContext, typesRegistry,
                    new EventsOutbox(compositeContext, reader),
                    loggerFactory.CreateLogger<EventsWriter>())
            );
           
            var backgroundPublisher = new OutboxPublisherService(
                new OutboxPublisher(
                    settings.Store.Connections,
                    settings.Store.Contexts,
                    reader),
                settings.Store.OutboxInterval,
                loggerFactory.CreateLogger<OutboxPublisherService>());
            
            services
                .AddAggregates(settings.AggregateFactories)
                .AddStreamResolver(settings.Store.Serializer)
                .AddSingleton<IEventStore>(eventStore)
                .AddSingleton<IHostedService>(backgroundPublisher);

            if (settings.Snapshots == null) 
                return services;
            
            return services.AddSnapshotStore(
                settings.Snapshots, 
                settings.Store.Connections,
                compositeContext, loggerFactory);
        }

        private static IServiceCollection AddSnapshotStore(
            this IServiceCollection services,
            SnapshotsSettings settings,
            DbConnections connections,
            IEventStoreContext context,
            ILoggerFactory loggerFactory)
        {
            if (settings == null)
                return services;
            
            if (settings.Serializer == null)
                throw new ArgumentException("Snapshot serializer not specified.");
            
            if (settings.TriggerResolver == null)
                throw new ArgumentException("ThresholdResolver not specified.");
            
            var snapshotStore = new SnapshotStore(
                connections, 
                context, 
                settings.Serializer);

            var loggingStore = new LoggingSnapshotStore(
                snapshotStore, 
                loggerFactory.CreateLogger<LoggingSnapshotStore>());
            
            services.AddSingleton<ISnapshotStore>(loggingStore);
            services.AddSingleton(settings.Serializer);
            services.AddSingleton(settings.TriggerResolver);

            return services;
        }

        private static IServiceCollection AddAggregates(
            this IServiceCollection services,
            IReadOnlyDictionary<Type, object> factories)
        {
            if (factories == null || !factories.Any())
                throw new InvalidOperationException("Factories not specified.");

            foreach (var (key, value) in factories)
            {
                if (value is Type type)
                    services.AddSingleton(key, type);
                else
                    services.AddSingleton(key, value);
            }

            return services;
        }

        private static IServiceCollection AddStreamResolver(
            this IServiceCollection services, IEventSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            
            services
                .AddSingleton<IAggregateStreamIdResolver, AggregateStreamIdResolver>()
                .AddSingleton(typeof(IEventSerializer), serializer);

            return services;
        }
        
        private static void InitializeSchemas(
            StoreSettings settings,
            EventTypeRegistry typesRegistry)
        {
            var schemas = settings.Contexts
                .SelectMany(c => c.Value.GetSchemas(c.Key))
                .ToArray();

            using var connection = new NpgsqlConnection(settings.Connections.Default);
            connection.Open();

            var installer = new SchemaInstaller(connection);
            foreach (var schema in schemas)
            {
                installer.Install(schema);
            }
                
            typesRegistry.Load(connection, schemas);
        }
    }
}