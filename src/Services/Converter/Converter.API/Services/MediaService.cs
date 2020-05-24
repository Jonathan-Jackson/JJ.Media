﻿using Converter.API.Converter;
using Converter.API.Models;
using Discord.API.Client.Client;
using System;
using System.IO;
using System.Text.Json;
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

        public async Task ConvertEpisode(string filePath, int episodeId, bool burnSubtitles) {
            await Convert(filePath, burnSubtitles);
            await _discordClient.AlertOfEpisode(episodeId);
        }

        public async Task Convert(string filePath, bool burnSubtitles) {
            if (!File.Exists(filePath))
                throw new IOException($"File does not exist: {filePath}");

            var fileInfo = new FileInfo(filePath);

            // we copy to a temp path because our CLI may not like
            // the characters in the original path.
            string tmpPath = await CreateTempFileCopy(fileInfo);
            var settings = new ConvertSettings { FilePath = tmpPath, BurnSubtitles = burnSubtitles };

            try {
                string output = await RetryConvert(settings);
                string destination = Path.Join(fileInfo.Directory.FullName, fileInfo.Name.Replace(fileInfo.Extension, ".webm"));
                await IORetry(() => File.Move(output, destination));
                //await IORetry(() => File.Delete(filePath));
            }
            finally {
                await IORetry(() => File.Delete(tmpPath));
            }
        }

        private async Task<string> RetryConvert(ConvertSettings settings) {
            for (int i = 0; ; i++) {
                string output = await _converter.Convert(settings);
                if (File.Exists(output))
                    return output;
                if (i >= IORetryCount)
                    throw new IOException($"Failed to convert file: {JsonSerializer.Serialize(settings)}");
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