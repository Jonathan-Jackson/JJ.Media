using JJ.Media.Core.Entities;
using System;

namespace Storage.Domain.Helpers.DTOs {

    public class ProcessedEpisode : Entity {
        public string Source { get; set; } = string.Empty;

        public string Output { get; set; } = string.Empty;

        public int EpisodeId { get; set; }

        public DateTimeOffset ProcessedOn { get; set; } = DateTimeOffset.UtcNow;

        public Guid Guid { get; set; } = Guid.NewGuid();
    }
}