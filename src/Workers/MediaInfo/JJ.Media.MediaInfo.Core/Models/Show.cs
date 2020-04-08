using JJ.Media.Core.Entities;
using System;
using System.Collections.Generic;

namespace JJ.Media.MediaInfo.Core.Models {

    public class Show : Entity {

        public Show() {
            Titles = new List<string>();
        }

        public List<string> Titles {
            get; set;
        }

        public int TvDbId {
            get; set;
        }
    }
}