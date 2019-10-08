namespace Amethyst.EventStore.Abstractions.Storage
{
    public interface IEventTypeRegistry
    {
        short GetOrAddTypeId(string typeName, StreamId streamId);
        
        string GetTypeName(short id, StreamId streamId);
    }
}