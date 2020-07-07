using Converter.Core.Converters;
using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Converter.Core.Services {

    public class ConverterService {
        private readonly ILogger<ConverterService> _log;
        private readonly IMessageBroker _broker;
        private readonly IConverter _converter;

        private readonly string _downloadStore;
        private readonly string _processingStore;
        private readonly string _queueStore;

        private const int ProcessQueueStoreInterval = 50_000; // 50 seconds.

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

        private async Task ProcessFileQueue() {
            for (; ; await Task.Delay(ProcessQueueStoreInterval)) {
                try {
                    // check folder store
                    // if in subtitled folder, burn subtitles
                    // if in nonsubtitled folder, dont burn subtitles
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the file queue!");
                }
            }
        }

        private async Task ProcessBrokerQueue() {
            for (; ; await Task.Delay(5000)) {
                try {
                    await _broker.RecieverAsync<string[]>("ConverterQueue",
                                                        (files) => ProcessFiles(files, true));
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the broker queue!");
                }
            }
        }

        private async Task ProcessFiles(IEnumerable<string> files, bool burnSubtitles) {
            var foundFiles = files.Where(file => File.Exists(file)).ToArray();
            // move to our processing store.

            // add delay to ensure lock is removed.
            await _converter.Convert(foundFiles, burnSubtitles);

            // finally blow up with an error for any that were missing..
            // we do this last to let the ones that do exist process.
            var missingFiles = files.Except(foundFiles);
            if (missingFiles.Any())
                throw new IOException($"Files missing for processing: {string.Join(", ", missingFiles)}");
        }

        private void SetupBroker() {
            _broker.DeclareQueue("ConverterQueue");
            _broker.DeclareExchange("DownloadedMedia");
            _broker.BindQueue("DownloadedMedia", "ConverterQueue", "FilePath");
        }
    }
}