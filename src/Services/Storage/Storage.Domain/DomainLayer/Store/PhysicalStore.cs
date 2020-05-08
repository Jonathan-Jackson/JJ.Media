using Storage.Domain.Helpers.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Storage.Domain.DomainLayer.Store {

    public class PhysicalStore {
        protected const long MinimumSpaceThresholdBytes = 150_000_000_00; // 10gb.
        protected readonly ImmutableArray<string> _downloadPaths;

        public PhysicalStore(DownloadStorageOptions downloadOptions) {
            _downloadPaths = ImmutableArray.Create(downloadOptions.Paths);
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
    }
}