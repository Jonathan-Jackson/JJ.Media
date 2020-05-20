using Converter.API.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Converter.API.Converter {

    public class HandbrakeConverter : IMediaConverter {
        private const string ConvertExtension = "webm";
        private const int ProcessTimeout = 7_200_000;
        private readonly string _cmdPath;
        private readonly string _cmdArgs;
        private readonly TaskCompletionSource<bool> _processWaiter;

        public HandbrakeConverter(HandbrakeOptions options) {
            _cmdArgs = options.CmdArgs;
            _cmdPath = options.CmdPath;
            _processWaiter = new TaskCompletionSource<bool>();
        }

        public async Task<string> Convert(string filePath) {
            string outputPath = filePath.Substring(0, filePath.LastIndexOf('.')) + "." + ConvertExtension;

            var info = new ProcessStartInfo() {
                FileName = _cmdPath,
                Arguments = $"{_cmdArgs} -i {filePath} -o {outputPath}",
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
    }
}