using JJ.HostedService;
using JJ.HostedService.Abstraction;
using Microsoft.Extensions.Logging;

namespace Converter.API.Hosted {

    public class BackgroundConvertHostedService : QueuedHostedService {

        public BackgroundConvertHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
            : base(taskQueue, logger, nameof(BackgroundConvertHostedService)) {
        }
    }
}