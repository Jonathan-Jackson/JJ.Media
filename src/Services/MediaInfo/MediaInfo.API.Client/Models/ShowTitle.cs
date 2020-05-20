namespace MediaInfo.API.Client.Models {

    public class ShowTitle {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        public int ShowId { get; set; }
    }
}