namespace Amethyst.EventStore.Abstractions.Metadata
{
    public interface IMetadataSerializer
    {
        byte[] Serialize(EventMetadata meta);

        EventMetadata Deserialize(byte[] bytes);
    }
}