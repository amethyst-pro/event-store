using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Publishing
{
    public interface ISendingOperation
    {
        Task SendAsync();
    }
}