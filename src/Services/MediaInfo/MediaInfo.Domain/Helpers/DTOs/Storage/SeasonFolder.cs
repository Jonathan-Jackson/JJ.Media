using System;

namespace MediaInfo.Domain.Helpers.DTOs.Storage {

    public class SeasonFolder {

        public SeasonFolder() {
            Media = Array.Empty<string>();
            Path = string.Empty;
        }

        public string Path {
            get; set;
        }

        public string[] Media {
            get; set;
        }

        public int Season {
            get; set;
        }
    }
}