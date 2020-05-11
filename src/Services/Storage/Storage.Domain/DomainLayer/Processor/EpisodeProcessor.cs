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
        private readonly IProcessedRepository _processedRepository;
        private readonly EventInvoker<ProcessedEpisode> _events;

        public EpisodeProcessor(IEpisodeStore episodeStore, ILogger<EpisodeProcessor> logger,
                IMediaInfoRepository mediaInfoRepository, IProcessedRepository processedRepository,
                EventInvoker<ProcessedEpisode> events) {
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
                var destination = await _episodeStore.SaveDownload(path, GetFolderPath(episode), CreateFileName(path, episode));
                await _processedRepository.InsertAsync(new ProcessedHistory { Type = eProcessedType.Episode, Source = path, Output = destination });
                _logger.LogInformation($"Processed Episode - FROM: {path} | TO: {destination}");
                await _events.InvokeAsync(new ProcessedEpisode(episode));
            }
            else {
                _logger.LogWarning($"Did not process '{episodeFileName}' as it was not found by the media information service. Full path: {path}");
            }
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