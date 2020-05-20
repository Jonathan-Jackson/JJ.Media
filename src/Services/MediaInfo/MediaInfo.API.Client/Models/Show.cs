using System;
using System.Collections.Generic;

namespace MediaInfo.API.Client.Models {

    public class Show {
        public int Id { get; set; }

        public DateTimeOffset? AirDate { get; set; }

        public string Overview { get; set; }

        public List<ShowTitle> Titles { get; set; } = new List<ShowTitle>();

        public int TvDbId { get; set; }
    }
}