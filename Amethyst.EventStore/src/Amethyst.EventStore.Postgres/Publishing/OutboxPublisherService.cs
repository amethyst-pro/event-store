using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.EventStore.Postgres.Publishing
{
    public sealed class OutboxPublisherService : BackgroundService
    {
        private readonly OutboxPublisher _publisher;
        private readonly TimeSpan _interval;
        private readonly ILogger<OutboxPublisherService> _logger;

        public OutboxPublisherService(
            OutboxPublisher publisher, 
            TimeSpan interval,
            ILogger<OutboxPublisherService> logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _interval = interval;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _publisher.PublishAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox publishing failed");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}