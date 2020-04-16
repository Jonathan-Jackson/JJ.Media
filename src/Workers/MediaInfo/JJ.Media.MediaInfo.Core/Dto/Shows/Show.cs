using JJ.Media.Core.Entities;
using System;
using System.Collections.Generic;

namespace JJ.Media.MediaInfo.Core.Entities {

    public class Show : Entity {

        public Show() {
            Titles = new List<ShowTitle>();
            Overview = string.Empty;
        }

        public List<ShowTitle> Titles {
            get; set;
        }

        public int TvDbId {
            get; set;
        }

        public string Overview { get; set; }
        public DateTimeOffset? AirDate { get; set; }
    }
}