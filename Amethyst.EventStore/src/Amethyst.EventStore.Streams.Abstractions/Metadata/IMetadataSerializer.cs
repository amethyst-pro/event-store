namespace Amethyst.EventStore.Streams.Abstractions.Metadata
{
    public interface IMetadataSerializer
    {
        byte[] Serialize(EventMetadata meta);

        EventMetadata Deserialize(byte[] bytes);
    }
}