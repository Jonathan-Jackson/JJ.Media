using JJ.HostedService.Abstraction;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.HostedService {

    public abstract class QueuedHostedService<TInput> : BackgroundService {
        private readonly ILogger<QueuedHostedService<TInput>> _logger;
        private readonly string _serviceName;

        public QueuedHostedService(IBackgroundTaskQueue<TInput> taskQueue, ILogger<QueuedHostedService<TInput>> logger, string serviceName) {
            TaskQueue = taskQueue;
            _logger = logger;
            _serviceName = serviceName;
        }

        public IBackgroundTaskQueue<TInput> TaskQueue { get; }

        public override async Task StopAsync(CancellationToken stoppingToken) {
            _logger.LogInformation($"{_serviceName} is stopping.");

            await base.StopAsync(stoppingToken);
        }

        protected virtual async Task ProcessQueue(TInput input, CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try {
                    await workItem(input, stoppingToken);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, $"Error occurred executing a background action ({nameof(workItem)}).");
                }
            }
        }
    }
}