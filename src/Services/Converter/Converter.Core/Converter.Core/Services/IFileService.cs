using Converter.Core.Helpers.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Converter.Core.Services {
    public interface IFileService {
        Task ProcessFile(string file, eMediaType mediaType);
        Task ProcessFiles(IEnumerable<string> files, eMediaType mediaType);
        Task ProcessQueueStore();
    }
}