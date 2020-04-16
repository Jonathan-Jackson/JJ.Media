using JJ.Media.Core.Entities;

namespace JJ.Media.MediaInfo.Core.Entities {

    public class ShowTitle : Entity {

        public ShowTitle() {
            Name = string.Empty;
        }

        public string Name {
            get; set;
        }

        public bool IsPrimary {
            get; set;
        }
    }
}