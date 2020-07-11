using Converter.Core.Helpers.Options;
using JJ.Framework.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Converter.Core.Converters {

    public class HandbrakeConverter : IConverter {
        private const string ConvertExtension = "webm";
        private const int ProcessTimeout = 7_200_000;

        private readonly string _subtitleArgs;
        private readonly string _standardArgs;
        private readonly string _cmdPath;
        private readonly ILogger<HandbrakeConverter> _log;

        private readonly SemaphoreSlim _semaphore; // Limit 1 file at a time for conversion.

        public HandbrakeConverter(HandbrakeOptions options, ILogger<HandbrakeConverter> log) {
            _subtitleArgs = options.SubtitleArgs;
            _standardArgs = options.StandardArgs;
            _cmdPath = options.CmdPath;
            _log = log;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Convert(string filePath, string outputDirectory, bool burnSubtitles) {
            if (!File.Exists(filePath))
                throw new IOException($"File cannot be found for processing: {filePath}");
            if (!Directory.Exists(outputDirectory))
                throw new IOException($"Directory for output cannot be found: {outputDirectory}");

            string output = string.Empty;

            try {
                await _semaphore.WaitAsync(TimeSpan.FromHours(3));
                output = await StartProcess(filePath, outputDirectory, burnSubtitles);
            }
            finally {
                _semaphore.Release();
            }

            // Await here to allow the processor to have fully
            // saved the file before checking.
            await Task.Delay(500);

            if (!File.Exists(output))
                throw new IOException("Failure converting file (Not Found).");
            if (GetFileLength(output) < (GetFileLength(filePath) / 8))
                throw new IOException("Failure converting file (Corrupt output detected).");
        }

        private long GetFileLength(string filePath)
            => new FileInfo(filePath).Length;

        private async Task<string> StartProcess(string file, string outputDirectory, bool burnSubtitles) {
            string outputPath = Path.Join(outputDirectory, $"{Path.GetFileNameWithoutExtension(file)}.{ConvertExtension}");
            await FileHelper.DeleteExistingFileWithRetryAsync(outputPath, _log);

            var info = new ProcessStartInfo() {
                FileName = _cmdPath,
                Arguments = $"{GetSettingArgs(burnSubtitles)} -i {file} -o {outputPath}",
                CreateNoWindow = true
            };

            // MS Docs note that processes should be
            // disposed, so we should await for it to finish rather
            // than fire & forget.
            var processWaiter = new TaskCompletionSource<bool>();
            using (var processTemp = new Process()) {
                processTemp.StartInfo = info;
                processTemp.Start();

                processTemp.EnableRaisingEvents = true;
                processTemp.Exited += (_, __) => processWaiter.TrySetResult(true);

                await Task.WhenAny(processWaiter.Task, Task.Delay(ProcessTimeout));
            }

            return outputPath;
        }

        private string GetSettingArgs(bool burnSubtitles)
            => burnSubtitles
            ? _subtitleArgs
            : _standardArgs;
    }
}