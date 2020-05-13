namespace MediaViewer.Web.Models {

    public class IndexViewModel {
        public string Path { get; set; }

        public bool IsValid
            => !string.IsNullOrWhiteSpace(Path);
    }
}