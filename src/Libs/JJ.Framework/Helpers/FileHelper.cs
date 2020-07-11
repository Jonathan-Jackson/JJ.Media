using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JJ.Framework.Helpers {

    public static class FileHelper {

        public static void DeleteExistingFile(string path) {
            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        public static Task DeleteExistingFileWithRetryAsync(string path, ILogger log = null) {
            return RetryIOAsync(
                    action: () => DeleteExistingFile(path),
                    fileLogInfo: path,
                    log: log);
        }

        public static Task CopyFileWithRetryAsync(string source, string output, ILogger log = null) {
            return RetryIOAsync(
                        action: () => CopyAsync(source, output),
                        fileLogInfo: source,
                        log: log);
        }

        public static Task MoveFileWithRetryAsync(string source, string output, ILogger log = null) {
            return RetryIOAsync(
                        action: () => File.Move(source, output),
                        fileLogInfo: source,
                        log: log);
        }

        public static async Task CopyAsync(string sourceFile, string destinationFile) {
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
            using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                await sourceStream.CopyToAsync(destinationStream)
                                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task RetryIOAsync(Action action, string fileLogInfo, int retryCount = 5, ILogger log = null) {
            for (int attempt = 1; ; attempt++) {
                try {
                    action();
                    return;
                }
                catch (Exception ex) {
                    if (attempt < retryCount) {
                        await Task.Delay(500 * attempt);
                        log?.LogWarning(ex, $"Failed Attempt {attempt} for moving file: {fileLogInfo}");
                    }
                    else {
                        log?.LogError(ex, $"FAILURE! Could not perform IO action on: {fileLogInfo}");
                        throw;
                    }
                }
            }
        }

        public static async Task RetryIOAsync(Func<Task> action, string fileLogInfo, int retryCount = 5, ILogger log = null) {
            for (int attempt = 1; ; attempt++) {
                try {
                    await action();
                    return;
                }
                catch (Exception ex) {
                    if (attempt < retryCount) {
                        await Task.Delay(500 * attempt);
                        log?.LogWarning(ex, $"Failed Attempt {attempt} for moving file: {fileLogInfo}");
                    }
                    else {
                        log?.LogError(ex, $"FAILURE! Could not perform IO action on: {fileLogInfo}");
                        throw;
                    }
                }
            }
        }
    }
}