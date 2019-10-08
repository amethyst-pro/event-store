using System.Collections.Generic;
using Amethyst.EventStore.Abstractions.Publishing;

namespace Amethyst.EventStore.Abstractions.Storage
{
    public interface IEventStoreContext
    {
        string GetSchema(StreamId stream);
        
        IReadOnlyCollection<string> GetSchemas(string category);
        
        IEventPublisher GetPublisher(StreamId stream);
    }
}