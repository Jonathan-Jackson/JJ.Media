using JJ.Media.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaInfo.Domain.Helpers.DTOs.Shows {

    public class Show : Entity {

        public Show() {
            Titles = new List<ShowTitle>();
            Overview = string.Empty;
        }

        public DateTimeOffset? AirDate { get; set; }

        public string Overview { get; set; }

        public List<ShowTitle> Titles {
            get; set;
        }

        public int TvDbId {
            get; set;
        }

        public string GetPrimaryTitle()
            => Titles.FirstOrDefault(x => x.IsPrimary)?.Title ?? Titles.FirstOrDefault()?.Title ?? string.Empty;

        public bool AddUniqueTitle(string title) {
            bool toAdd = !Titles.Select(x => x.Title).Any(x => string.Equals(title, x, StringComparison.OrdinalIgnoreCase));

            if (toAdd) {
                Titles.Add(new ShowTitle { ShowId = Id, Title = title, IsPrimary = !Titles.Any() });
            }

            return toAdd;
        }
    }
}