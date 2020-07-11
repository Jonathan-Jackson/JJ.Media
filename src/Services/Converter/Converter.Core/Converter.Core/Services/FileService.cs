using Converter.Core.Converters;
using Converter.Core.Helpers.Enums;
using Converter.Core.Helpers.Options;
using JJ.Framework.Helpers;
using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Converter.Core.Services {

    public class FileService : IFileService {
        private readonly IMessageBroker _broker;
        private readonly IConverter _converter;
        private readonly ILogger<FileService> _log;
        private readonly ConverterOptions _options;

        public FileService(ConverterOptions options, IMessageBroker broker, IConverter converter, ILogger<FileService> log) {
            _options = options;
            _broker = broker;
            _converter = converter;
            _log = log;
        }

        public async Task ProcessFile(string file, eMediaType mediaType) {
            if (!File.Exists(file))
                throw new IOException($"File Path Missing: {file}");

            string fileName = Path.GetFileName(file);
            string processingPath = Path.Join(_options.ProcessingStore, fileName);

            await FileHelper.DeleteExistingFileWithRetryAsync(processingPath, _log);
            await FileHelper.CopyFileWithRetryAsync(file, processingPath, _log);

            // add delay to ensure any file lock is removed.
            await Task.Delay(1000);
            await _converter.Convert(processingPath, _options.ProcessedStore, burnSubtitles: mediaType == eMediaType.Anime);

            _broker.Publish(mediaType.ToString(), fileName, "ConvertedMedia");

            // Clean up the ProcessingFolder + the original origin of the file.
            await Task.WhenAll(
                FileHelper.DeleteExistingFileWithRetryAsync(processingPath, _log),
                FileHelper.DeleteExistingFileWithRetryAsync(file, _log));
        }

        public async Task ProcessFiles(IEnumerable<string> files, eMediaType mediaType) {
            // Process each file individually.
            // A file failure should not impact the others.
            foreach (var file in files) {
                try {
                    await ProcessFile(file, mediaType);
                }
                catch (Exception ex) {
                    _log.LogError(ex, $"Failed to process: {file}");
                }
            }
        }

        public async Task ProcessQueueStore() {
            // check folder store
            // we dont use a folder watch to allow for re-tries.
            var subtitleFiles = Directory.EnumerateFiles(GetAnimeFolderPath());
            await ProcessFiles(GetConvertableFiles(subtitleFiles), eMediaType.Anime);

            var nonSubtitleFiles = Directory.EnumerateFiles(GetShowFolderPath());
            await ProcessFiles(GetConvertableFiles(nonSubtitleFiles), eMediaType.Show);
        }

        private string GetAnimeFolderPath()
            => Path.Join(_options.QueueStore, "anime");

        private IEnumerable<string> GetConvertableFiles(IEnumerable<string> source)
                    => source.Where(file => file.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase)
                        || file.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase));

        private string GetShowFolderPath()
            => Path.Join(_options.QueueStore, "shows");
    }
}