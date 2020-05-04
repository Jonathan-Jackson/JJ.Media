using JJ.Media.Core.Entities;

namespace MediaInfo.Domain.Helpers.DTOs.Shows {

    public class ShowTitle : Entity {

        public ShowTitle() {
            Title = string.Empty;
        }

        public string Title {
            get; set;
        }

        public bool IsPrimary {
            get; set;
        }

        public int ShowId {
            get; set;
        }
    }
}