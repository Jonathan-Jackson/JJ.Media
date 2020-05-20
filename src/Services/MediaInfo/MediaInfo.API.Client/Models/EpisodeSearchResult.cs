namespace MediaInfo.API.Client.Models {

    public class EpisodeSearchResult {
        public int Id { get; set; }

        public int ShowId { get; set; }

        public string ShowTitle { get; set; } = string.Empty;

        public string EpisodeTitle { get; set; } = string.Empty;

        public int? SeasonNumber { get; set; }

        public int? EpisodeNumber { get; set; }

        public int? AbsoluteNumber { get; set; }

        public string Overview { get; set; } = string.Empty;
    }
}