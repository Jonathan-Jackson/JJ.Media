using Microsoft.Extensions.DependencyInjection;
using Storage.Domain.Helpers.Options;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Storage.IntegrationTesting.Controller {

    public class MediaControllerProcessTests : ControllerTestBase {

        [Theory]
        [InlineData("[PineappleSubs] Kaguya-sama wa Kokurasetai S2 - 05 [1080p].mkv")]
        [InlineData("One Piece - 05.mkv")]
        [InlineData("[HorribleSubs] Tsugumomo S2 - 05 [1080p].mkv")]
        public async Task MockFile(string fileName) {
            string fullPath = CreateMockFile(fileName);
            var response = await _client.PostAsync("/api/media/process", new StringContent($"\"{fileName}\"", Encoding.UTF8, "application/json"));
            Assert.True(response.IsSuccessStatusCode);
            Assert.False(File.Exists(fullPath));
        }

        [Fact]
        public async Task MissingFile() {
            var response = await _client.PostAsync("/api/media/process", new StringContent($"\"dsafafdsf\"", Encoding.UTF8, "application/json"));
            Assert.False(response.IsSuccessStatusCode);
        }

        private string CreateMockFile(string fileName) {
            // Create a fake file in the
            var downloader = _services.GetRequiredService<DownloadStorageOptions>();
            string path = downloader.Paths.First();
            string fullPath = Path.Combine(path, fileName);

            if (!File.Exists(fullPath))
                File.Create(fullPath)
                    .Dispose();

            return fullPath;
        }
    }
}