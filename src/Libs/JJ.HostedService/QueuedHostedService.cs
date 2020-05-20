using JJ.HostedService.Abstraction;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.HostedService {

    public class QueuedHostedService : BackgroundService {
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly string _serviceName;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger, string serviceName) {
            TaskQueue = taskQueue;
            _logger = logger;
            _serviceName = serviceName;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        public override async Task StopAsync(CancellationToken stoppingToken) {
            _logger.LogInformation($"{_serviceName} is stopping.");

            await base.StopAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            _logger.LogInformation($"{_serviceName} Started.");

            await ProcessQueue(stoppingToken);
        }

        protected virtual async Task ProcessQueue(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try {
                    await workItem(stoppingToken);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, $"Error occurred executing a background action ({nameof(workItem)}).");
                }
            }
        }
    }
}