using System;

namespace JJ.Media.MediaInfo.Core.Models {

    public class MinedEpisode {

        public MinedEpisode() {
            PossibleNames = Array.Empty<string>();
        }

        public string[] PossibleNames {
            get; set;
        }

        public uint SeasonNumber {
            get; set;
        }

        public uint EpisodeNumber {
            get; set;
        }
    }
}