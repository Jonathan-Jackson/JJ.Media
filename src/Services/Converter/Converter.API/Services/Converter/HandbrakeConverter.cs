using Converter.API.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Converter.API.Converter {

    public class HandbrakeConverter : IMediaConverter {
        private const string ConvertExtension = "webm";
        private const int ProcessTimeout = 7_200_000;

        private readonly string _subtitleArgs;
        private readonly string _standardArgs;
        private readonly string _cmdPath;
        private readonly TaskCompletionSource<bool> _processWaiter;

        public HandbrakeConverter(HandbrakeOptions options) {
            _subtitleArgs = options.SubtitleArgs;
            _standardArgs = options.StandardArgs;
            _cmdPath = options.CmdPath;
            _processWaiter = new TaskCompletionSource<bool>();
        }

        public async Task<string> Convert(ConvertSettings settings) {
            string outputPath = settings.FilePath.Substring(0, settings.FilePath.LastIndexOf('.')) + "." + ConvertExtension;

            var info = new ProcessStartInfo() {
                FileName = _cmdPath,
                Arguments = $"{GetSettingArgs(settings)} -i {settings.FilePath} -o {outputPath}",
                CreateNoWindow = true
            };

            // MS Docs note that processes should be
            // disposed, so we should await for it to finish rather
            // than fire & forget.
            using (var processTemp = new Process()) {
                processTemp.StartInfo = info;
                processTemp.Start();

                processTemp.EnableRaisingEvents = true;
                processTemp.Exited += (_, __) => _processWaiter.TrySetResult(true);

                await Task.WhenAny(_processWaiter.Task, Task.Delay(ProcessTimeout));
            }

            return outputPath;
        }

        private string GetSettingArgs(ConvertSettings settings)
            => settings.BurnSubtitles
            ? _subtitleArgs
            : _standardArgs;
    }
}