using System;

namespace Storage.Domain.Helpers.Options {

    public class DownloadStorageOptions {
        public string[] Paths { get; set; } = Array.Empty<string>();
    }
}