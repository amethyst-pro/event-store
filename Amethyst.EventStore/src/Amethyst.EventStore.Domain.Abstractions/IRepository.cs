using System.Threading.Tasks;
using SharpJuice.Essentials;

namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface IRepository<TAggregate, in TId>
        where TAggregate : IAggregate<TId>
    {
        Task<Maybe<TAggregate>> GetAsync(TId id);
        
        Task SaveAsync(TAggregate aggregate);
        
        Task<bool> ExistsAsync(TId id);
    }
}