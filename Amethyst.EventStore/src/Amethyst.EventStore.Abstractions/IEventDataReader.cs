using System.IO;

namespace Amethyst.EventStore.Abstractions
{
    public interface IEventDataReader
    {
        Stream GetData();
        Stream GetMetadata();

        byte[] GetRawData();
        byte[] GetRawMetadata();
    }
}