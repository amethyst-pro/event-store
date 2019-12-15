using System;
using System.Collections.Generic;
using Amethyst.EventStore.Abstractions.Serialization;
using Amethyst.EventStore.Domain.Abstractions;
using Amethyst.EventStore.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.EventStore.Hosting.Settings
{
    public sealed class AggregateSettingsBuilder
    {
        private readonly EventStoreSettingsBuilder _builder = new EventStoreSettingsBuilder();

        public AggregateSettingsBuilder AddAggregate<T, TId>(
            IServiceCollection services, 
            Func<TId, long, IReadOnlyCollection<IDomainEvent>, T> factory,
            string topic)
            where T : IAggregate<TId>
            where TId : IGuidId
        {
            _builder.AddAggregate(services, factory, topic);

            return this;
        }
        
        public AggregateSettingsBuilder AddAggregate<T, TId, TFactory>(
            IServiceCollection services,
            string topic,
            int partitions = 1)
            where T : IAggregate<TId>
            where TId : IGuidId
            where TFactory : IAggregateFactory<T, TId>
        {
            _builder.AddAggregate<T, TId, TFactory>(services, topic, partitions);

            return this;
        }

        public SnapshotSettingsBuilder AddSnapshotableAggregate<T, TId>(
            IServiceCollection services, 
            Func<TId, long, IReadOnlyCollection<IDomainEvent>, T> factory,
            string topic,
            int triggerThreshold)
            where T : ISnapshotableAggregate<TId>
            where TId : IGuidId
        {
            _builder.AddAggregate(services, factory, topic);
            _builder.UseSnapshots<T, TId>(services, triggerThreshold);

            return new SnapshotSettingsBuilder(_builder);
        }

        public SnapshotSettingsBuilder AddSnapshotableAggregate<T, TId, TFactory>(
            IServiceCollection services,
            string topic,
            int triggerThreshold,
            int partitions = 1)
            where T : ISnapshotableAggregate<TId>
            where TId : IGuidId
            where TFactory : IAggregateFactory<T, TId>
        {
            _builder.AddAggregate<T, TId, TFactory>(services, topic, partitions);
            _builder.UseSnapshots<T, TId>(services, triggerThreshold);

            return new SnapshotSettingsBuilder(_builder);
        }
        
        public EventStoreSettings Build(
            DbConnections connections,
            IPublisherFactory publisherFactory,
            IEventSerializer eventSerializer)
        {
            return _builder.Build(connections, publisherFactory, eventSerializer);
        }
    }
}