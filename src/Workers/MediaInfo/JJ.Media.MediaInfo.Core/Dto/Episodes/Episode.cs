using JJ.Media.Core.Entities;

namespace JJ.Media.MediaInfo.Core.Entities.Episodes {

    public class Episode : Entity {

        public Episode() {
            Title = string.Empty;
            Overview = string.Empty;
        }

        public string Title { get; set; }
        public int? AbsoluteNumber { get; set; }
        public string Overview { get; set; }
        public bool IsMovie { get; set; }
        public int? SeasonNumber { get; set; }
        public int TvDbId { get; set; }
        public int? EpisodeNumber { get; set; }
    }
}