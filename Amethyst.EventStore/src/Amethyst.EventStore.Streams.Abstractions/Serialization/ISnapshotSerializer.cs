using System.IO;
using Amethyst.EventStore.Domain.Abstractions;

namespace Amethyst.EventStore.Streams.Abstractions.Serialization
{
    public interface ISnapshotSerializer
    {
        byte[] Serialize<T, TId>(IAggregateSnapshot snapshot)
            where T: IAggregate<TId>;
        
        IAggregateSnapshot Deserialize<T, TId>(Stream stream)
            where T: IAggregate<TId>;
    }
}