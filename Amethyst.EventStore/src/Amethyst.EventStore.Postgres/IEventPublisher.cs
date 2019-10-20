using System.Collections.Generic;
using System.Threading.Tasks;
using Amethyst.EventStore.Abstractions;

namespace Amethyst.EventStore.Postgres
{
    public interface IEventPublisher
    {
        Task PublishAsync(IReadOnlyCollection<RecordedEvent> events);
    }
}