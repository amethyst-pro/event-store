using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Storage
{
    public interface IEventSender
    {
        Task SendAsync();
    }
}