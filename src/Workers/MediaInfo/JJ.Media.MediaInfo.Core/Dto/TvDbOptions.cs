namespace JJ.Media.MediaInfo.Core.Dto {

    public class TvDbOptions {

        public TvDbOptions() {
            UserKey = string.Empty;
            Username = string.Empty;
            ApiKey = string.Empty;
        }

        public string Username {
            get; set;
        }

        public string ApiKey {
            get; set;
        }

        public string UserKey {
            get; set;
        }
    }
}