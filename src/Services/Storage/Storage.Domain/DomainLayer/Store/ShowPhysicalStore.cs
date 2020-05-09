using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.Options;
using System.IO;
using System.Linq;

namespace Storage.Domain.DomainLayer.Store {

    public class ShowPhysicalStore : PhysicalStore, IShowStore {

        public ShowPhysicalStore(DownloadStorageOptions downloadOptions, MediaStorageOptions mediaOptions)
            : base(downloadOptions, mediaOptions) {
        }

        public bool HasShowFolder(string primaryTitle)
            => _storePaths
                .Select(path => Path.Combine(path, primaryTitle))
                .Any(Directory.Exists);
    }
}