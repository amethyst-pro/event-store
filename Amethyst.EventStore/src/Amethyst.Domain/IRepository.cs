using System.Threading.Tasks;
using SharpJuice.Essentials;

namespace Amethyst.Domain
{
    public interface IRepository<TAggregate, in TId>
        where TAggregate : IAggregate<TId>
    {
        Task<Maybe<TAggregate>> Get(TId id);
        
        Task Save(TAggregate aggregate);
        
        Task<bool> Exists(TId id);
    }
}