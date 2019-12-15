using System;
using System.Collections.Generic;
using System.Linq;
using Amethyst.EventStore.Abstractions.Publishing;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Abstractions.Storage;
using Amethyst.EventStore.Domain;
using Amethyst.EventStore.Domain.Abstractions;
using Amethyst.EventStore.Postgres;
using Amethyst.EventStore.Postgres.Configuration;
using Amethyst.EventStore.Postgres.Context;
using Amethyst.EventStore.Streams.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Amethyst.EventStore.Hosting.Settings
{
    public sealed class EventStoreSettingsBuilder
    {
        private readonly TriggerThresholdResolver _thresholdResolver = new TriggerThresholdResolver();
        
        private readonly List<RegisteredAggregate> _aggregates = 
            new List<RegisteredAggregate>();
        
        public EventStoreSettingsBuilder AddAggregate<T, TId>(
            IServiceCollection services,
            Func<TId, long, IReadOnlyCollection<IDomainEvent>, T> factory, 
            string topic)
            where T : IAggregate<TId>
            where TId : IGuidId
        {
            var aggregate = new RegisteredAggregate(
                typeof(T),
                typeof(IAggregateFactory<T, TId>), 
                new AggregateFactory<T, TId>(factory), 
                topic, 
                1);
            
            services.AddSingleton(
                typeof(IRepository<T,TId>),
                typeof(EventSourcedRepository<T,TId>));

            _aggregates.Add(aggregate);
            
            return this;
        }
        
        public EventStoreSettingsBuilder AddAggregate<T, TId, TFactory>(
            IServiceCollection services,
            string topic, 
            int partitions = 1)
            where T : IAggregate<TId>
            where TId : IGuidId
            where TFactory: IAggregateFactory<T, TId>
        {
            if (partitions < 1)
                throw new InvalidOperationException("Partitions must be more then one.");

            var aggregate = new RegisteredAggregate(
                typeof(T), 
                typeof(IAggregateFactory<T, TId>), 
                typeof(TFactory), 
                topic,
                partitions);
            
            services.AddSingleton(
                typeof(IRepository<T,TId>),
                typeof(EventSourcedRepository<T,TId>));
            
            _aggregates.Add(aggregate);

            return this;
        }

        public EventStoreSettingsBuilder UseSnapshots<T, TId>(
            IServiceCollection services,
            int triggerThreshold)
            where T : ISnapshotableAggregate<TId>
            where TId : IGuidId
        {
            var descriptor = new ServiceDescriptor(
                typeof(IRepository<T,TId>),
                typeof(SnapshotableRepository<T,TId>),
                ServiceLifetime.Singleton);
            
             services.Replace(descriptor);
             
             _thresholdResolver.AddTriggerThreshold<T>(triggerThreshold);

             return this;
        }
        
        public EventStoreSettings Build(
            DbConnections connections,
            IPublisherFactory publisherFactory,
            IEventSerializer eventSerializer)
        {
            if (connections == null)
                throw new ArgumentNullException(nameof(connections));
            
            if (publisherFactory == null)
                throw new ArgumentNullException(nameof(publisherFactory));
            
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));
            
            var contexts = new Dictionary<string, IEventStoreContext>();
            _aggregates.ForEach(a =>
            {
                if (a.Partitions > 1)
                {
                    var (type, context) = CreateCategoryType(a.Type, a.Partitions, publisherFactory.Create(a.Topic));
                    contexts.Add(type, context);
                }

                else
                {
                    var (type, context) = CreateCategoryType(a.Type, publisherFactory.Create(a.Topic));
                    contexts.Add(type, context);
                }
            });
            
            var storeSettings = new StoreSettings(connections, contexts, eventSerializer);
            var aggregateFactories = _aggregates.ToDictionary(k => k.FactoryType, v => v.Factory);

            return new EventStoreSettings(storeSettings, aggregateFactories);
        }
        
        public EventStoreSettings Build(
            DbConnections connections,
            IPublisherFactory publisherFactory,
            IEventSerializer eventSerializer,
            ISnapshotSerializer snapshotSerializer)
        {
            if (snapshotSerializer == null)
                throw new ArgumentNullException(nameof(snapshotSerializer));

            var tempSettings = Build(connections, publisherFactory, eventSerializer);
            
            var snapshotsSettings = new SnapshotsSettings(_thresholdResolver, snapshotSerializer);
            
            return new EventStoreSettings(
                tempSettings.Store, 
                tempSettings.AggregateFactories, 
                snapshotsSettings);
        }
        
        private static (string type, IEventStoreContext context) CreateCategoryType(
            Type type, 
            int partitions, 
            IEventPublisher publisher)
        {
            if (type.IsGenericType) 
                throw new ArgumentException("Generic types are not supported.");

            return (type.Name, new MultiPartitionContext(partitions, publisher));
        }
        
        private static (string type, IEventStoreContext context) CreateCategoryType(
            Type type, 
            IEventPublisher publisher)
        {
            if (type.IsGenericType)
                throw new ArgumentException("Generic types are not supported.");

            return (type.Name, new SinglePartitionContext(publisher));
        }
    }
}