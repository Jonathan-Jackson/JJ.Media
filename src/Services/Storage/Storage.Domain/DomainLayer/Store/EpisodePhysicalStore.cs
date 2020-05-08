using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.Options;
using System.Collections.Immutable;
using System.IO;

namespace Storage.Domain.DomainLayer.Store {

    public class EpisodePhysicalStore : PhysicalStore, IEpisodeStore {
        private readonly ImmutableArray<string> _storePaths;
        private readonly ILogger<EpisodePhysicalStore> _logger;

        public EpisodePhysicalStore(ILogger<EpisodePhysicalStore> logger, EpisodeStorageOptions episodeOptions, DownloadStorageOptions downloadOptions)
                : base(downloadOptions) {
            _logger = logger;
            _storePaths = ImmutableArray.Create(episodeOptions.Paths);
        }

        public string SaveDownload(string sourceFile, string folderPath, string fileName) {
            string source = GetDownloadPath(sourceFile);
            string outputPath = CreateOutputPath(folderPath, fileName);

            if (File.Exists(outputPath)) {
                _logger.LogInformation($"File already exists. Deleting existing copy of {fileName} at path: {outputPath}");
                File.Delete(outputPath);
            }

            File.Move(source, outputPath);
            return outputPath;
        }

        private string CreateOutputPath(string folderPath, string fileName) {
            string storePath = GetAvailablePath(_storePaths);

            string storeFolder = string.Concat(Path.Combine(storePath, folderPath).Split(Path.GetInvalidPathChars()));
            Directory.CreateDirectory(storeFolder);

            string sanitizedFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(storeFolder, sanitizedFileName);
        }
    }
}