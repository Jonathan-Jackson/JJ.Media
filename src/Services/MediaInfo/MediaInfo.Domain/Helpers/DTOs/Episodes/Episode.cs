using JJ.Media.Core.Entities;

namespace MediaInfo.Domain.Helpers.DTOs.Episodes {

    public class Episode : Entity {
        public string Title { get; set; } = string.Empty;
        public int? AbsoluteNumber { get; set; }
        public string Overview { get; set; } = string.Empty;
        public bool IsMovie { get; set; }
        public int? SeasonNumber { get; set; }
        public int TvDbId { get; set; }
        public int? EpisodeNumber { get; set; }
        public int ShowId { get; set; }
    }
}