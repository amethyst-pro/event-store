using System.Threading.Tasks;

namespace Amethyst.EventStore.Abstractions.Publishing
{
    public interface IProducer
    {
        Task ProduceAsync(byte[] @event, byte[] key = null);
    }
}