using System.IO;

namespace Amethyst.EventStore.Abstractions.Reading
{
    public interface IEventDataReader
    {
        Stream GetData();
        Stream GetMetadata();

        byte[] GetRawData();
        byte[] GetRawMetadata();
    }
}