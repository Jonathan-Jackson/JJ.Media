﻿using Converter.API.Client.Client;
using MediaInfo.API.Client.Client;
using MediaInfo.API.Client.Models;
using Microsoft.Extensions.Logging;
using Storage.Domain.DomainLayer.Namer;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Exceptions;
using Storage.Domain.Helpers.Repository;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Processor {

    public class EpisodeProcessor : Processor {
        private readonly IEpisodeStore _episodeStore;
        private readonly MediaInfoClient _mediaInfoRepository;
        private readonly IProcessedEpisodeRepository _repo;
        private readonly ConverterClient _converterClient;

        public EpisodeProcessor(IEpisodeStore episodeStore, ILogger<EpisodeProcessor> logger,
                MediaInfoClient mediaInfoClient, IProcessedEpisodeRepository processedRepository,
                ConverterClient converterClient)
            : base(logger) {
            _episodeStore = episodeStore;
            _mediaInfoRepository = mediaInfoClient;
            _repo = processedRepository;
            _converterClient = converterClient;
        }

        /// <summary>
        /// Processes the episode file passed into storage.
        /// </summary>
        public override async Task ProcessAsync(string path) {
            string episodeFileName = GetFileName(path);
            EpisodeSearchResult episode = await _mediaInfoRepository.EpisodeSearch(episodeFileName);

            if (episode.Id > 0) {
                await ProcessFoundEpisodeAsync(path, episode);
            }
            else {
                _logger.LogWarning($"Did not process '{episodeFileName}' as it was not found by the media information service. Show Id: {episode.ShowId} / Full path: {path}");
                throw new EpisodeNotFoundException();
            }
        }

        private async Task ProcessFoundEpisodeAsync(string path, EpisodeSearchResult episode) {
            var namer = new EpisodeNamer(path, episode);
            var finalDestination = await _episodeStore.SaveDownload(path, namer.FolderPath, namer.FileName);

            // Log.
            var processedEpisode = new ProcessedEpisode { EpisodeId = episode.Id, Source = path, Output = finalDestination };
            await UpdateOrAddToRepo(processedEpisode);

            // Notify other services - i.e. discord messages.
            _logger.LogInformation($"Processed Episode - FROM: {path} | TO: {finalDestination}");
            await TrySendNotifications(processedEpisode);
        }

        private async Task UpdateOrAddToRepo(ProcessedEpisode processedEpisode) {
            var existingRecord = await _repo.FindByEpisodeAsync(processedEpisode.EpisodeId);
            if (existingRecord != null) {
                processedEpisode.Id = existingRecord.Id;
                await _repo.UpdateAsync(processedEpisode);
            }
            else {
                processedEpisode.Id = await _repo.InsertAsync(processedEpisode);
            }
        }

        private async Task TrySendNotifications(ProcessedEpisode processedEpisode) {
            try {
                await _converterClient.EpisodeToWebm(processedEpisode.Output, processedEpisode.EpisodeId);
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Failed to notify the converter service of: {JsonSerializer.Serialize(processedEpisode)}");
            }
        }
    }
}