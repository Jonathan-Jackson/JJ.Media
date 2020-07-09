using Converter.Core.Converters;
using Converter.Core.Helpers.Enums;
using JJ.Framework.Helpers;
using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Converter.Core.Services {

    public class ConverterService {
        private readonly IMessageBroker _broker;
        private readonly IConverter _converter;
        private readonly ILogger<ConverterService> _log;

        private readonly string _processingStore;
        private readonly string _queueStore;
        private readonly string _downloadStore;

        private const int ProcessQueueStoreInterval = 50_000; // 50 seconds.
        private const string ConverterQueue = "ConverterQueue";

        public async Task Run() {
            _log.LogInformation("Converter Service Ran...");
            SetupBroker();

            while (true) {
                try {
                    _log.LogInformation("Awaiting Event..");
                    await Task.WhenAll(ProcessBrokerQueue(),
                                        ProcessFileQueue());
                }
                catch (Exception ex) {
                    _log.LogError(ex, "Exception thrown when executing the main program");
                }

                await Task.Delay(5_000);
            }
        }

        private IEnumerable<string> GetConvertableFiles(IEnumerable<string> source)
            => source.Where(file => file.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase)
                                || file.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase));

        private string GetShowFolderPath()
            => Path.Join(_queueStore, "shows");

        private string GetAnimeFolderPath()
            => Path.Join(_queueStore, "anime");

        private async Task ProcessBrokerQueue() {
            for (; ; await Task.Delay(5000)) {
                try {
                    await _broker.RecieverAsync<string[]>(ConverterQueue, (files) => ProcessFiles(files, eMediaType.Anime));
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the broker queue!");
                }
            }
        }

        private async Task ProcessFileQueue() {
            for (; ; await Task.Delay(ProcessQueueStoreInterval)) {
                try {
                    // check folder store
                    // we dont use a folder watch to allow for re-tries.
                    var subtitleFiles = Directory.EnumerateFiles(GetAnimeFolderPath());
                    await ProcessFiles(GetConvertableFiles(subtitleFiles), eMediaType.Anime);

                    var nonSubtitleFiles = Directory.EnumerateFiles(GetShowFolderPath());
                    await ProcessFiles(GetConvertableFiles(nonSubtitleFiles), eMediaType.Show);
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the file queue!");
                }
            }
        }

        private async Task ProcessFiles(IEnumerable<string> files, eMediaType mediaType) {
            var foundFiles = files.Where(file => File.Exists(file)).ToArray();

            foreach (var file in foundFiles) {
                string outputPath = Path.Join(_processingStore, file);

                await FileHelper.DeleteExistingFileWithRetryAsync(outputPath, _log);
                await FileHelper.CopyFileWithRetryAsync(Path.Join(_downloadStore, file), outputPath, _log);

                // add delay to ensure any file lock is removed.
                await Task.Delay(1000);
                await _converter.Convert(Path.Join(_processingStore, file), burnSubtitles: mediaType == eMediaType.Anime);
            }

            // finally blow up with an error for any that were missing..
            // we do this last to let the ones that do exist process.
            var missingFiles = files.Except(foundFiles);
            if (missingFiles.Any())
                throw new IOException($"Files missing for processing: {string.Join(", ", missingFiles)}");
        }

        private void SetupBroker() {
            _broker.DeclareQueue(ConverterQueue);
            _broker.DeclareExchange("DownloadedMedia");
            _broker.BindQueue("DownloadedMedia", ConverterQueue, "FilePath");
        }
    }
}