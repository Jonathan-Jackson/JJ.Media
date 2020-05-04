using System;

namespace MediaInfo.Domain.Helpers.DTOs.Miners {

    public class MinedEpisode {
        public string[] PossibleNames { get; set; } = Array.Empty<string>();

        public int? SeasonNumber { get; set; }

        public int EpisodeNumber { get; set; }

        public string Source { get; set; } = string.Empty;
    }
}