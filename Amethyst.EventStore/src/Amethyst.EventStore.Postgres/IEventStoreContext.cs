using System.Collections.Generic;
using Amethyst.EventStore.Postgres.Publishing;

namespace Amethyst.EventStore.Postgres
{
    public interface IEventStoreContext
    {
        string GetSchema(StreamId stream);
        IReadOnlyCollection<string> GetSchemas(string category);
        IEventPublisher GetPublisher(StreamId stream);
    }
}