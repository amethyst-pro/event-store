using System.IO;

namespace Amethyst.Metadata
{
    public interface IMetadataSerializer
    {
        byte[] Serialize(EventMetadata meta);

        EventMetadata Deserialize(byte[] bytes);

        EventMetadata? Deserialize(Stream data);
    }
}