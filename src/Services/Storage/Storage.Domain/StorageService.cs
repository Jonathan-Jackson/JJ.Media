using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using Storage.Domain.DomainLayer.Processor;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Exceptions;
using System;
using System.Threading.Tasks;

namespace Storage.Domain.Services {

    public class StorageService {
        private readonly ILogger<StorageService> _log;
        private readonly IMessageBroker _broker;
        private readonly EpisodeProcessor _episodeProcessor;

        private const string StorageQueue = "StorageQueue";

        public StorageService(ILogger<StorageService> log, IMessageBroker broker, EpisodeProcessor episodeProcessor) {
            _log = log;
            _broker = broker;
            _episodeProcessor = episodeProcessor;
        }

        public async Task Run() {
            _log.LogInformation("Storage Service Ran..");
            SetupBroker();

            while (true) {
                try {
                    _log.LogInformation($"Awaiting broker messages on: {StorageQueue}");
                    await _broker.RecieverAsync(StorageQueue, ProcessFile);
                }
                catch (Exception ex) {
                    _log.LogError(ex, "Fatal error awaiting broker message");
                    await Task.Delay(5000);
                }
            }
        }

        private async Task ProcessFile(string file) {
            try {
                _log.LogInformation($"Processing File: {file}");
                eMediaType media = GetMediaTypeFromPath(file);

                if (media == eMediaType.Anime || media == eMediaType.Show)
                    await _episodeProcessor.ProcessAsync(file, media);
                else
                    throw new NotImplementedException($"Media type not supported yet: {media} (Path: {file})");
            }
            catch (EpisodeNotFoundException) {
                // Already logged further down. We can go on freely.
            }
            catch (Exception ex) {
                _log.LogError(ex, "Error thrown while processing a file to the media store.");
            }
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
            _broker.BindQueue("ConvertedMedia", StorageQueue, "anime");
            _broker.BindQueue("ConvertedMedia", StorageQueue, "shows");
            _broker.BindQueue("ConvertedMedia", StorageQueue, "movies");
        }
    }
}