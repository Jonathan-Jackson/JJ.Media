namespace Storage.Domain.Helpers.DTOs {

    public class EpisodeSearch {

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