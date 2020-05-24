namespace Converter.API.Models {

    public class EpisodeFileRequest {
        public string FilePath { get; set; } = string.Empty;

        public int EpisodeId { get; set; }

        public bool BurnSubtitles { get; set; }
    }
}