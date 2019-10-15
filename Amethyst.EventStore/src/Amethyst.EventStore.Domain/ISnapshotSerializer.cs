using System.IO;
using Amethyst.Domain;

namespace Amethyst.EventStore.Domain.Abstractions
{
    public interface ISnapshotSerializer
    {
        byte[] Serialize<T, TId>(IAggregateSnapshot snapshot)
            where T: IAggregate<TId>;
        
        IAggregateSnapshot Deserialize<T, TId>(Stream stream)
            where T: IAggregate<TId>;
    }
}