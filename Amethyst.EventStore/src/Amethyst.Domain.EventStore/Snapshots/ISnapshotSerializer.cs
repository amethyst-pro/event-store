using System.IO;

namespace Amethyst.Domain.EventStore.Snapshots
{
    public interface ISnapshotSerializer
    {
        byte[] Serialize<T, TId>(IAggregateSnapshot snapshot)
            where T: IAggregate<TId>;
        
        IAggregateSnapshot Deserialize<T, TId>(Stream stream)
            where T: IAggregate<TId>;
    }
}