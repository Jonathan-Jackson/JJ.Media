using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Storage.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.API.HostedService {

    public class StorageHostedService : IHostedService, IDisposable {
        private readonly ILogger<StorageHostedService> _logger;
        private readonly StorageService _storageService;

        public StorageHostedService(ILogger<StorageHostedService> logger) {
            _logger = logger;
        }

        public StorageHostedService(ILogger<StorageHostedService> logger, StorageService storageService)
            : this(logger) {
            _storageService = storageService;
        }

        public async Task StartAsync(CancellationToken stoppingToken) {
            _logger.LogInformation("Storage Hosted Service running.");
            await _storageService.Run(stoppingToken);
        }

        public Task StopAsync(CancellationToken stoppingToken) {
            _logger.LogInformation("Storage Hosted Service is stopping.");
            return Task.CompletedTask;
        }

        public void Dispose() {
        }
    }
}