using System.Collections.Generic;

namespace Amethyst.EventStore.Postgres
{
    public interface IEventStoreContext
    {
        int GetPartitionsCount(string category);
        string GetSchema(StreamId stream);
        string GetSchema(string category, int partition);
        int GetPartition(StreamId stream);
        IReadOnlyCollection<string> GetSchemas(string category);
        IEventPublisher GetPublisher(StreamId stream);
    }
}