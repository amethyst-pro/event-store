namespace Amethyst.EventStore.Abstractions
{
    public readonly struct WriteResult
    {
        public WriteResult(long nextExpectedVersion)
            => NextExpectedVersion = nextExpectedVersion;
        
        public long NextExpectedVersion { get; }
    }
}