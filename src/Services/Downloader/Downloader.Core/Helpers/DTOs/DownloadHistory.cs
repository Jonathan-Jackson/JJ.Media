using JJ.Framework.Repository;
using System;

namespace Downloader.Core.Helpers.DTOs {

    public class DownloadHistory : Entity {
        public string MagnetUri { get; set; } = string.Empty;

        public DateTimeOffset DownloadedOn { get; set; } = DateTimeOffset.UtcNow;

        public string Title { get; set; } = string.Empty;
    }
}