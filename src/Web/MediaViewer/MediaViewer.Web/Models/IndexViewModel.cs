namespace MediaViewer.Web.Models {

    public class IndexViewModel {
        public int EpisodeId { get; set; }
        public string Path { get; set; }

        public bool IsValid
            => !string.IsNullOrWhiteSpace(Path);
    }
}