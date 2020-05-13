using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Events;
using Storage.Domain.Helpers.Repository;
using System.IO;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Processor {

    public class EpisodeProcessor : Processor {
        private const int MaxFileNameLength = 100;

        private readonly IEpisodeStore _episodeStore;
        private readonly ILogger<EpisodeProcessor> _logger;
        private readonly IMediaInfoRepository _mediaInfoRepository;
        private readonly IProcessedEpisodeRepository _processedRepository;
        private readonly EventInvoker<ProcessedEpisodeEvent> _events;

        public EpisodeProcessor(IEpisodeStore episodeStore, ILogger<EpisodeProcessor> logger,
                IMediaInfoRepository mediaInfoRepository, IProcessedEpisodeRepository processedRepository,
                EventInvoker<ProcessedEpisodeEvent> events) {
            _episodeStore = episodeStore;
            _logger = logger;
            _mediaInfoRepository = mediaInfoRepository;
            _processedRepository = processedRepository;
            _events = events;
        }

        /// <summary>
        /// Processes the episode file passed into storage.
        /// </summary>
        public override async Task ProcessAsync(string path) {
            string episodeFileName = GetFileName(path);
            EpisodeSearch episode = await _mediaInfoRepository.SearchEpisode(episodeFileName);

            if (episode.Id > 0) {
                await ProcessFoundEpisodeAsync(path, episode);
            }
            else {
                _logger.LogWarning($"Did not process '{episodeFileName}' as it was not found by the media information service. Show Id: {episode.ShowId} / Full path: {path}");
                // throw exception? unproc. entity http result?
            }
        }

        private async Task ProcessFoundEpisodeAsync(string path, EpisodeSearch episode) {
            var destination = await _episodeStore.SaveDownload(path, GetFolderPath(episode), CreateFileName(path, episode));
            var processInfo = new ProcessedEpisode { EpisodeId = episode.Id, Source = path, Output = destination };
            await _processedRepository.InsertAsync(processInfo);

            _logger.LogInformation($"Processed Episode - FROM: {path} | TO: {destination}");
            await _events.InvokeAsync(new ProcessedEpisodeEvent(episode, processInfo));
        }

        private string GetFolderPath(EpisodeSearch episode)
            // Default to Anime (since thats only whats supported atm).
            => Path.Combine("Anime", episode.ShowTitle, $"Season {episode.SeasonNumber}");

        private string CreateFileName(string path, EpisodeSearch episode) {
            if (HasLargeName(episode) || string.IsNullOrWhiteSpace(episode.EpisodeTitle)) {
                return $"{episode.ShowTitle} - S{FormattedSeasonNumber(episode)}E{FormattedEpisodeNumber(episode)}{GetFileExtension(path)}";
            }
            else {
                return $"{episode.ShowTitle} - S{FormattedSeasonNumber(episode)}E{FormattedEpisodeNumber(episode)} ({episode.EpisodeTitle}){GetFileExtension(path)}";
            }
        }

        private string FormattedEpisodeNumber(EpisodeSearch episode)
            => episode.EpisodeNumber < 10 ? $"0{episode.EpisodeNumber}" : episode.EpisodeNumber.ToString();

        private string FormattedSeasonNumber(EpisodeSearch episode)
            => episode.SeasonNumber < 10 ? $"0{episode.SeasonNumber}" : episode.SeasonNumber.ToString();

        private string GetFileExtension(string path)
            => new FileInfo(path).Extension;

        private bool HasLargeName(EpisodeSearch episode)
            => (episode?.EpisodeTitle?.Length ?? 0 + episode.ShowTitle.Length) >= MaxFileNameLength;
    }
}