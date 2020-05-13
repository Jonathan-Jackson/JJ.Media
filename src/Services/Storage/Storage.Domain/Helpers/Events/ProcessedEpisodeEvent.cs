using Storage.Domain.Helpers.DTOs;
using System;

namespace Storage.Domain.Helpers.Events {

    public class ProcessedEpisodeEvent {

        public ProcessedEpisodeEvent() {
        }

        public ProcessedEpisodeEvent(EpisodeSearch episode, ProcessedEpisode processInfo) {
            EpisodeId = episode.Id;
            ShowId = episode.ShowId;
            ShowTitle = episode.ShowTitle ?? string.Empty;
            EpisodeTitle = episode.EpisodeTitle ?? string.Empty;
            EpisodeNumber = episode.EpisodeNumber ?? 0;
            SeasonNumber = episode.SeasonNumber ?? 0;
            Guid = processInfo.Guid;
        }

        public int EpisodeId { get; set; }
        public int ShowId { get; set; }
        public string ShowTitle { get; set; }
        public string EpisodeTitle { get; set; }
        public int EpisodeNumber { get; set; }
        public int SeasonNumber { get; set; }
        public Guid Guid { get; set; }
    }
}