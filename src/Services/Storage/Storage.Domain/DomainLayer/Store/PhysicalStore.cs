using Storage.Domain.Helpers.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Store {

    public class PhysicalStore {
        protected const long MinimumSpaceThresholdBytes = 150_000_000_00; // 10gb.
        protected readonly ImmutableArray<string> _downloadPaths;
        protected readonly ImmutableArray<string> _storePaths;

        protected static char[] InvalidPathCharacters
            => Path.GetInvalidPathChars()
            // ^ does not return all invalid path characters!
            .Concat(new[] { '*', ':', '"', '>', '<', '?' })
            .ToArray();

        public PhysicalStore(DownloadStorageOptions downloadOptions, MediaStorageOptions mediaOptions) {
            _downloadPaths = ImmutableArray.Create(downloadOptions.Paths);
            _storePaths = ImmutableArray.Create(mediaOptions.Paths);
        }

        protected string GetAvailablePath(IEnumerable<string> paths) {
            var drives = DriveInfo.GetDrives();

            foreach (string path in paths) {
                if (drives.Any(drive => drive.IsReady && path.StartsWith(drive.Name) && drive.AvailableFreeSpace > MinimumSpaceThresholdBytes)) {
                    return path;
                }
            }

            // A suitable drive not found.. Throw a suitable error.
            if (paths.Any(path => !drives.Any(drive => path.StartsWith(drive.Name)))) {
                string path = paths.First(path => !drives.Any(drive => path.StartsWith(drive.Name)));
                throw new IOException($"A path is not routed to a local drive, so it cannot be checked for available space: {path}. To resolve this change the path to be a drive path i.e. {@"D:\\Shows"}");
            }
            else {
                throw new InsufficientMemoryException($"There are no free storage locations provided by the storage settings. The last {MinimumSpaceThresholdBytes / 1000000000} of a drive are reserved.");
            }
        }

        protected string GetDownloadPath(string source) {
            foreach (string path in _downloadPaths) {
                string filePath = Path.Combine(path, source);
                if (File.Exists(filePath))
                    return filePath;
            }

            throw new IOException($"File does not exist in downloads: {source}");
        }

        protected async Task RetryFileMove(string source, string target, int attempts) {
            for (int i = 0; ; i++) {
                try {
                    File.Move(source, target);
                    break;
                }
                catch (IOException) {
                    if (i >= attempts)
                        throw;
                    else
                        await Task.Delay(1000 * i);
                }
            }
        }
    }
}