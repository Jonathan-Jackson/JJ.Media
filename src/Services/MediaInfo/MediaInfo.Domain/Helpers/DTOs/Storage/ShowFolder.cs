using System;

namespace MediaInfo.Domain.Helpers.DTOs.Storage {

    public class ShowFolder {

        public ShowFolder() {
            Seasons = Array.Empty<SeasonFolder>();
            Images = Array.Empty<string>();
            Path = string.Empty;
        }

        public SeasonFolder[] Seasons {
            get; set;
        }

        public string[] Images {
            get; set;
        }

        public string Path {
            get; set;
        }
    }
}