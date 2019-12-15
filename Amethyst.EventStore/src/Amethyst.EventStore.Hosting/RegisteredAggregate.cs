using System;

namespace Amethyst.EventStore.Hosting
{
    public readonly struct RegisteredAggregate
    {
        public RegisteredAggregate(
            Type type, 
            Type factoryType, 
            object factory, 
            string topic, 
            int partitions)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            FactoryType = factoryType ?? throw new ArgumentNullException(nameof(factoryType));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            
            if (string.IsNullOrEmpty(topic)) 
                throw new ArgumentException("Topic not specified.", nameof(topic));
            
            if (partitions < 0)
                throw new ArgumentException("Partitions not specified.", nameof(topic));
            
            Topic = topic;
            Partitions = partitions;
        }
        
        public Type Type { get; }

        public Type FactoryType { get; }

        public object Factory { get; }
        
        public string Topic { get; }
        
        public int Partitions { get; }
    }
}