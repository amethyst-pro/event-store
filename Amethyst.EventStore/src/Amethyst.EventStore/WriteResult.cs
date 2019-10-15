namespace Amethyst.EventStore
{
    public readonly struct WriteResult
    {
        public readonly long NextExpectedVersion;

        public WriteResult(long nextExpectedVersion)
        {
            NextExpectedVersion = nextExpectedVersion;
        }
    }
}