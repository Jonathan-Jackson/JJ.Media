using Converter.API.Converter;
using Converter.API.Models;
using System.IO;
using System.Threading.Tasks;

namespace Converter.API.Services {

    public class MediaService {
        private readonly IMediaConverter _converter;
        private readonly string _tempPath;

        public MediaService(IMediaConverter converter, ConverterOptions options) {
            _converter = converter;
            _tempPath = options.TempPath;
        }

        public async Task Convert(string filePath) {
            var fileInfo = new FileInfo(filePath);

            // we copy to a temp path because our CLI may not like
            // the characters in the original path.
            string tmpPath = Path.Combine(_tempPath, $"{filePath.GetHashCode()}{fileInfo.Extension}");
            File.Copy(filePath, tmpPath);

            try {
                string output = await _converter.Convert(tmpPath);

                // IOs have delays to allow
                // windows to remove file handles.
                await Task.Delay(5000);
                string destination = Path.Join(fileInfo.Directory.FullName, fileInfo.Name.Replace(fileInfo.Extension, ".webm"));
                File.Move(output, destination);
            }
            finally {
                await Task.Delay(5000);
                File.Delete(tmpPath);
            }
        }
    }
}