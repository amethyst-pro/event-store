using System.Threading.Tasks;

namespace Amethyst.EventStore.Postgres
{
    public interface IEventSendingOperation
    {
        Task SendAsync();
    }
}