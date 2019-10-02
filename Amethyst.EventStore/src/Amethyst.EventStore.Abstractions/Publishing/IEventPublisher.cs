using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Publishing
{
    public interface IEventPublisher
    {
        Task PublishAsync(IReadOnlyCollection<RecordedEvent> events);
    }
}