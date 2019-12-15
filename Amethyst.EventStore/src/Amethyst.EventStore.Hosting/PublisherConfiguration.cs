namespace Amethyst.EventStore.Hosting
{
    public sealed class PublisherConfiguration
    {
        public PublisherConfiguration(string brokers)
            => Brokers = brokers;

        public string Brokers { get; }
    }
}