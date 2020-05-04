using MediaInfo.Domain.Helpers.DTOs.Episodes;
using MediaInfo.Domain.Helpers.DTOs.Shows;

namespace MediaInfo.API.ViewModels {

    public class EpisodeSearchResponse {

        public EpisodeSearchResponse(Episode episode, Show show) {
            Id = episode.Id;
            ShowId = show.Id;
            ShowTitle = show.GetPrimaryTitle() ?? string.Empty;
            EpisodeTitle = episode.Title ?? string.Empty;
            Overview = episode.Overview ?? string.Empty;
            EpisodeNumber = episode.EpisodeNumber;
            AbsoluteNumber = episode.AbsoluteNumber;
            SeasonNumber = episode.SeasonNumber;
        }

        public int Id {
            get; set;
        }

        public int ShowId {
            get; set;
        }

        public string ShowTitle {
            get; set;
        }

        public string EpisodeTitle {
            get; set;
        }

        public int? SeasonNumber {
            get; set;
        }

        public int? EpisodeNumber {
            get; set;
        }

        public int? AbsoluteNumber {
            get; set;
        }

        public string Overview {
            get; set;
        }
    }
}