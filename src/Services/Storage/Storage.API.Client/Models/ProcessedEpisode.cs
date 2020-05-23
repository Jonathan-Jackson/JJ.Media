using System;

namespace Storage.API.Client.Models {

    public class ProcessedEpisode {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public string Output { get; set; } = string.Empty;

        public int EpisodeId { get; set; }

        public DateTimeOffset ProcessedOn { get; set; }

        public Guid Guid { get; set; }
    }
}