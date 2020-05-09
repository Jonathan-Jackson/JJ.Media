using System;

namespace Storage.Domain.Helpers.Options {

    public class MediaStorageOptions {
        public string[] Paths { get; set; } = Array.Empty<string>();
    }
}