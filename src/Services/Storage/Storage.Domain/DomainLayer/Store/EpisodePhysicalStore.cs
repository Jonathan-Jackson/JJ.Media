using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.Options;
using System.IO;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Store {

    public class EpisodePhysicalStore : PhysicalStore, IEpisodeStore {
        private readonly ILogger<EpisodePhysicalStore> _logger;

        public EpisodePhysicalStore(ILogger<EpisodePhysicalStore> logger, DownloadStorageOptions downloadOptions, MediaStorageOptions mediaOptions)
                : base(downloadOptions, mediaOptions) {
            _logger = logger;
        }

        public async Task<string> SaveDownload(string sourceFile, string folderPath, string fileName) {
            string source = GetDownloadPath(sourceFile);
            string outputPath = CreateOutputPath(folderPath, fileName);

            if (File.Exists(outputPath)) {
                _logger.LogInformation($"File already exists. Deleting existing copy of {fileName} at path: {outputPath}");
                File.Delete(outputPath);
            }

            await RetryFileMove(source, outputPath, 5);
            return outputPath;
        }

        private string CreateOutputPath(string folderPath, string fileName) {
            string storePath = GetAvailablePath(_storePaths);

            string sanitizedFolderName = string.Concat(folderPath.Split(InvalidPathCharacters));
            string storeFolder = Path.Combine(storePath, sanitizedFolderName);
            Directory.CreateDirectory(storeFolder);

            string sanitizedFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(storeFolder, sanitizedFileName);
        }
    }
}