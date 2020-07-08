using Converter.Core.Helpers.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Converter.Core.Converters {

    public class HandbrakeConverter : IConverter {
        private const string ConvertExtension = "webm";
        private const int ProcessTimeout = 7_200_000;

        private readonly string _subtitleArgs;
        private readonly string _standardArgs;
        private readonly string _cmdPath;
        private readonly string _outputDirectory;

        private readonly SemaphoreSlim _semaphore; // Limit 1 file at a time for conversion.

        public HandbrakeConverter(HandbrakeOptions options) {
            _subtitleArgs = options.SubtitleArgs;
            _standardArgs = options.StandardArgs;
            _cmdPath = options.CmdPath;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Convert(IReadOnlyCollection<string> files, bool burnSubtitles) {
            // process files that dont exist last
            // so we can blow up for those.
            var priorityFiles = files.OrderByDescending(file => File.Exists(file));

            foreach (var file in priorityFiles)
                await Convert(file, burnSubtitles);
        }

        public async Task Convert(string file, bool burnSubtitles) {
            if (!File.Exists(file))
                throw new IOException($"File cannot be found for processing: {file}");

            string output = string.Empty;

            try {
                await _semaphore.WaitAsync(TimeSpan.FromHours(3));
                output = await StartProcess(file, burnSubtitles);
            }
            finally {
                _semaphore.Release();
            }

            if (!File.Exists(output))
                throw new IOException("Failure converting file (Not Found).");
            if (GetFileLength(output) < (GetFileLength(file) / 8))
                throw new IOException("Failure converting file (Corrupt output detected).");
        }

        private long GetFileLength(string filePath)
            => new FileInfo(filePath).Length;

        private async Task<string> StartProcess(string file, bool burnSubtitles) {
            string outputPath = Path.Combine(_outputDirectory, file.Substring(0, file.LastIndexOf('.')) + "." + ConvertExtension);

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