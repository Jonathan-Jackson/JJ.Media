using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using Storage.Domain.DomainLayer.Processor;
using Storage.Domain.Helpers.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Domain.Services {

    public class StorageService {
        private readonly ILogger<StorageService> _log;
        private readonly IMessageBroker _broker;
        private readonly EpisodeProcessor _episodeProcessor;

        private const string StorageQueue = "StorageQueue";

        public async Task Run(CancellationToken stoppingToken) {
            _log.LogInformation("Storage Service Ran..");
            SetupBroker();

            while (!stoppingToken.IsCancellationRequested) {
                try {
                    _log.LogInformation($"Awaiting broker messages on: {StorageQueue}");
                    await _broker.RecieverAsync<string>(StorageQueue, (file) => ProcessFile(file), stoppingToken);
                }
                catch (Exception ex) {
                    _log.LogError(ex, "Fatal error awaiting broker message");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private async Task ProcessFile(string file) {
            _log.LogInformation($"Processing File: {file}");
            eMediaType media = GetMediaTypeFromPath(file);

            if (media == eMediaType.Anime || media == eMediaType.Show)
                await _episodeProcessor.ProcessAsync(file, media);
            else
                throw new NotImplementedException($"Media type not supported yet: {media} (Path: {file})");
        }

        private eMediaType GetMediaTypeFromPath(string file) {
            if (file.StartsWith("anime", StringComparison.OrdinalIgnoreCase))
                return eMediaType.Anime;
            if (file.StartsWith("shows", StringComparison.OrdinalIgnoreCase))
                return eMediaType.Show;
            if (file.StartsWith("movies", StringComparison.OrdinalIgnoreCase))
                return eMediaType.Movie;

            throw new ArgumentException($"Unable to find the media type for path: {file}");
        }

        private void SetupBroker() {
            _broker.DeclareQueue(StorageQueue);
            _broker.DeclareExchange("ConvertedMedia");
            _broker.BindQueue("ConvertedMedia", StorageQueue, "Anime");
            _broker.BindQueue("ConvertedMedia", StorageQueue, "Shows");
            _broker.BindQueue("ConvertedMedia", StorageQueue, "Movies");
        }
    }
}