using System;

namespace Storage.Domain.Helpers.Options {

    public class EpisodeStorageOptions {
        public string[] Paths { get; set; } = Array.Empty<string>();
    }
}