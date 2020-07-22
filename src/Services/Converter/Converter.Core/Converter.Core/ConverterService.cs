using Converter.Core.Helpers.Enums;
using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Converter.Core.Services {

    public class ConverterService {
        private readonly ILogger<ConverterService> _log;
        private readonly IMessageBroker _broker;
        private readonly IFileService _fileService;

        private const int ProcessQueueStoreInterval = 50_000; // 50 seconds.
        private const int NotifyProcessedFilesInterval = 50_000; // 50 seconds.

        private const string ConverterQueue = "ConverterQueue";

        public ConverterService(ILogger<ConverterService> log, IMessageBroker broker, IFileService fileService) {
            _log = log;
            _broker = broker;
            _fileService = fileService;
        }

        public async Task Run() {
            _log.LogInformation("Converter Service Ran...");
            SetupBroker();

            while (true) {
                try {
                    _log.LogInformation("Awaiting Event..");
                    await Task.WhenAll(ProcessBrokerQueue(),
                                        ProcessFileQueue(),
                                        PushProcessedFiles());
                }
                catch (Exception ex) {
                    _log.LogError(ex, "Exception thrown when executing the main program");
                }

                await Task.Delay(5_000);
            }
        }

        private async Task PushProcessedFiles() {
            for (; ; await Task.Delay(NotifyProcessedFilesInterval)) {
                try {
                    _fileService.PushProcessedStore();
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the broker queue!");
                }
            }
        }

        private async Task ProcessBrokerQueue() {
            for (; ; await Task.Delay(5000)) {
                try {
                    await _broker.RecieverAsync<string[]>(ConverterQueue, (files) => _fileService.ProcessFiles(files, eMediaType.Anime));
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the broker queue!");
                }
            }
        }

        private async Task ProcessFileQueue() {
            for (; ; await Task.Delay(ProcessQueueStoreInterval)) {
                try {
                    await _fileService.ProcessQueueStore();
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the file queue!");
                }
            }
        }

        private void SetupBroker() {
            _broker.DeclareQueue(ConverterQueue);
            _broker.DeclareExchange("DownloadedMedia");
            _broker.BindQueue("DownloadedMedia", ConverterQueue, "FilePath");
        }
    }
}