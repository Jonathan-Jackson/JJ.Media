using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Store {

    public class EpisodePhysicalStore : PhysicalStore, IEpisodeStore {
        private readonly ILogger<EpisodePhysicalStore> _logger;

        public EpisodePhysicalStore(ILogger<EpisodePhysicalStore> logger, MediaStorageOptions mediaOptions)
                : base(mediaOptions) {
            _logger = logger;
        }

        public async Task<string> SaveDownload(string sourceFile, string folderPath, string fileName) {
            string source = GetProcessPath(sourceFile);
            string outputPath = CreateOutputPath(folderPath, fileName);

            // Return if our file is already at the destination
            // We still want to log these as processed so they're in the
            // database!
            if (string.Equals(source, outputPath, StringComparison.OrdinalIgnoreCase)) {
                return RemoveStorePath(outputPath);
            }
            else if (File.Exists(outputPath)) {
                _logger.LogInformation($"File already exists. Deleting existing copy of {fileName} at path: {outputPath}");
                File.Delete(outputPath);
            }

            await RetryFileMove(source, outputPath, 5);

            // We remove the physical path for logging
            // this allows us to have a 'generic' file path.
            return RemoveStorePath(outputPath);
        }

        private string RemoveStorePath(string path) {
            var store = _mediaStores.FirstOrDefault(store => path.StartsWith(store.Path));
            string storeRemoved = path.Substring(store.Path.Length, path.Length - store.Path.Length).TrimStart('\\');

            return store != null
                ? Path.Combine(store.Alias, storeRemoved)
                : path;
        }

        private string CreateOutputPath(string folderPath, string fileName) {
            string storePath = GetAvailablePath(_mediaStores);

            string sanitizedFolderName = string.Concat(folderPath.Split(InvalidPathCharacters));
            string storeFolder = Path.Combine(storePath, sanitizedFolderName);
            Directory.CreateDirectory(storeFolder);

            string sanitizedFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(storeFolder, sanitizedFileName);
        }
    }
}