using Converter.API.Converter;
using Converter.API.Models;
using Discord.API.Client.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Converter.API.Services {

    public class MediaService {
        private const int FileHandleTimer = 5000;
        private const int IORetryCount = 5;

        private readonly IMediaConverter _converter;
        private readonly string _tempPath;
        private readonly DiscordClient _discordClient;

        public MediaService(IMediaConverter converter, ConverterOptions options, DiscordClient discordClient) {
            _converter = converter;
            _tempPath = options.TempPath;
            _discordClient = discordClient;
        }

        public async Task ConvertEpisode(string filePath, int episodeId) {
            await Convert(filePath);
            await _discordClient.AlertOfEpisode(episodeId);
        }

        public async Task Convert(string filePath) {
            if (!File.Exists(filePath))
                throw new IOException($"File does not exist: {filePath}");

            var fileInfo = new FileInfo(filePath);

            // we copy to a temp path because our CLI may not like
            // the characters in the original path.
            string tmpPath = await CreateTempFileCopy(fileInfo);

            try {
                string output = await RetryConvert(tmpPath);
                string destination = Path.Join(fileInfo.Directory.FullName, fileInfo.Name.Replace(fileInfo.Extension, ".webm"));
                await IORetry(() => File.Move(output, destination));
            }
            finally {
                await IORetry(() => File.Delete(filePath));
            }
        }

        private async Task<string> RetryConvert(string tmpPath) {
            for (int i = 0; ; i++) {
                string output = await _converter.Convert(tmpPath);
                if (File.Exists(output))
                    return output;
                if (i >= IORetryCount)
                    throw new IOException($"Failed to convert file: {tmpPath}");
            }
        }

        private async Task<string> CreateTempFileCopy(FileInfo fileInfo) {
            string tmpPath = Path.Combine(_tempPath, $"{fileInfo.FullName.GetHashCode()}{fileInfo.Extension}");
            if (File.Exists(tmpPath))
                await IORetry(() => File.Delete(tmpPath));

            await IORetry(() => File.Copy(fileInfo.FullName, tmpPath));

            return tmpPath;
        }

        private async Task IORetry(Action action) {
            for (int i = 0; ; i++) {
                try {
                    action();
                    break;
                }
                catch (IOException) {
                    if (i >= IORetryCount) throw;
                    await Task.Delay(FileHandleTimer);
                }
            }
        }
    }
}