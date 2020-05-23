using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Options;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Store {

    public class PhysicalStore {
        protected const long MinimumSpaceThresholdBytes = 150_000_000_00; // 10gb.
        protected readonly ImmutableArray<StoreArea> _ProcessStores;
        protected readonly ImmutableArray<StoreArea> _mediaStores;

        protected static char[] InvalidPathCharacters
            => Path.GetInvalidPathChars()
            // ^ does not return all invalid path characters!
            .Concat(new[] { '*', ':', '"', '>', '<', '?' })
            .ToArray();

        public PhysicalStore(MediaStorageOptions mediaOptions) {
            _ProcessStores = ImmutableArray.Create(mediaOptions.ProcessStores);
            _mediaStores = ImmutableArray.Create(mediaOptions.Stores);
        }

        protected string GetAvailablePath(IEnumerable<StoreArea> paths)
            => paths.FirstOrDefault(x => x.Write)?.Path ?? throw new IOException("There are no stores available to write to.");

        protected string GetProcessPath(string source) {
            foreach (string path in _ProcessStores.Select(x => x.Path)) {
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
                        await Task.Delay(2000 * i);
                }
            }
        }
    }
}