using MediaInfo.Domain.Helpers.DTOs;
using MediaInfo.Domain.Helpers.DTOs.Storage;
using System;
using System.IO;
using System.Linq;

namespace JJ.Media.MediaInfo.Services {

    /// <summary>
    /// Returns storage information relating to a show.
    /// </summary>
    public class ShowStorage {
        private readonly MediaInfoConfiguration _config;

        public ShowStorage(MediaInfoConfiguration config) {
            _config = config;
        }

        /// <summary>
        /// Returns a show folder if found.
        /// </summary>
        public ShowFolder? FindShowFolder(string showName) {
            string escapedName = string.Join("", showName.Split(Path.GetInvalidFileNameChars()));

            foreach (string path in _config.StoragePaths)
                if (Directory.Exists(Path.Combine(path, escapedName)))
                    return GetFolder(Path.Combine(path, escapedName));

            return null;
        }

        /// <summary>
        /// Returns a folder for a show.
        /// </summary>
        private ShowFolder GetFolder(string path) {
            if (!Directory.Exists(path))
                throw new ArgumentException($"Folder does not exist: {path}");

            var directory = new DirectoryInfo(path);
            return new ShowFolder {
                Path = path,
                Seasons = directory.GetDirectories().Select(GetSeasonFolder).ToArray(),
                Images = directory.GetFiles().Where(IsImageFile).Select(x => x.Name).ToArray()
            };
        }

        /// <summary>
        /// Returns true if the file extension is that of an image.
        /// </summary>
        private bool IsImageFile(FileInfo file)
            => string.Equals(file.Extension, "png", StringComparison.OrdinalIgnoreCase)
            || string.Equals(file.Extension, "jpeg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(file.Extension, "jpg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(file.Extension, "gif", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///  Returns the season folder of a directory.
        /// </summary>
        private SeasonFolder GetSeasonFolder(DirectoryInfo directory) {
            int season = 0;
            if (directory.Name.Contains("Season", StringComparison.OrdinalIgnoreCase))
                int.TryParse(new string(directory.Name.Where(char.IsDigit).ToArray()), out season);
            
            return new SeasonFolder {
                Path = directory.FullName,
                Season = season,
                Media = directory.GetFiles().Select(x => x.Name).ToArray()
            };
        }
    }
}