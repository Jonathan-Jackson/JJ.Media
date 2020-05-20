namespace MediaViewer.Web.Models {

    public class IndexViewModel {
        public string Path { get; set; }

        public bool IsReady { get; set; }

        public bool IsDownloaded
            => !string.IsNullOrWhiteSpace(Path);
    }
}