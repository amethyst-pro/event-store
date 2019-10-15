using System.Threading.Tasks;

namespace Amethyst.EventStore.Postgres.Publishing
{
    public interface IEventSendingOperation
    {
        Task SendAsync();
    }
}