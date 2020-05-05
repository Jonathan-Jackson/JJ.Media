using System;

namespace Downloader.Core.Helpers.DTOs {

    public class Torrent {
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset PublishDate { get; set; }
        public string MagnetUri { get; set; } = string.Empty;
    }
}