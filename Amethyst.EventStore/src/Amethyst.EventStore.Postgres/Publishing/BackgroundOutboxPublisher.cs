using System;
using System.Threading;
using System.Threading.Tasks;
using Amethyst.EventStore.Postgres.Configurations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.EventStore.Postgres.Publishing
{
    public sealed class BackgroundOutboxPublisher : BackgroundService
    {
        private readonly OutboxPublisher _publisher;
        private readonly TimeSpan _interval;
        private readonly ILogger<BackgroundOutboxPublisher> _logger;

        public BackgroundOutboxPublisher(
            OutboxPublisher publisher, 
            StoreSettings settings,
            ILogger<BackgroundOutboxPublisher> logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _interval = settings.OutboxInterval;
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
