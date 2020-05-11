using Storage.Domain.Helpers.DTOs;

namespace Storage.Domain.Helpers.Events {

    public class ProcessedEpisode {

        public ProcessedEpisode(EpisodeSearch episode) {
            EpisodeId = episode.Id;
            ShowId = episode.ShowId;
            ShowTitle = episode.ShowTitle ?? string.Empty;
            EpisodeTitle = episode.EpisodeTitle ?? string.Empty;
            EpisodeNumber = episode.EpisodeNumber ?? 0;
            SeasonNumber = episode.SeasonNumber ?? 0;
        }

        public int EpisodeId { get; set; }
        public int ShowId { get; set; }
        public string ShowTitle { get; set; }
        public string EpisodeTitle { get; set; }
        public int EpisodeNumber { get; set; }
        public int SeasonNumber { get; set; }
    }
}