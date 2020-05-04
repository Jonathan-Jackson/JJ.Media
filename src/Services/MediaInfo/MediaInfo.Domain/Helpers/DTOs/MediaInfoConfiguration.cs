using System;

namespace MediaInfo.Domain.Helpers.DTOs {

    public class MediaInfoConfiguration {
        public string[] StoragePaths;

        public MediaInfoConfiguration() {
            StoragePaths = Array.Empty<string>();
        }
    }
}