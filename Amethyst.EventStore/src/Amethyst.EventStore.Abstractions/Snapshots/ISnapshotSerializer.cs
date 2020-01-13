using System.IO;

namespace Amethyst.EventStore.Abstractions.Snapshots
{
    public interface ISnapshotSerializer
    {
        byte[] Serialize<T>(in T snapshot);

        T Deserialize<T>(Stream byteStream);
    }
}