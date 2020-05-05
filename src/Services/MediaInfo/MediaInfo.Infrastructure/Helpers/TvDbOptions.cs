namespace MediaInfo.Infrastructure.Helpers.Models {

    /// <summary>
    /// Options for the TvDb API.
    /// </summary>
    public class TvDbOptions {

        public TvDbOptions() {
            Token = string.Empty;
        }

        public string Token {
            get; set;
        }
    }
}