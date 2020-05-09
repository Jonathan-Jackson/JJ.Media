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
        private const string MockFileName = "[HorribleSubs] Kaguya-sama wa Kokurasetai S2 - 05 [1080p].mkv";

        [Fact]
        public async Task ProcessMockFile() {
            string fileName = CreateMockFile();
            var response = await _client.PostAsync("/api/media/process", new StringContent($"\"{fileName}\"", Encoding.UTF8, "application/json"));
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ProcessMissingFile() {
            var response = await _client.PostAsync("/api/media/process", new StringContent($"\"dsafafdsf\"", Encoding.UTF8, "application/json"));
            Assert.False(response.IsSuccessStatusCode);
        }

        private string CreateMockFile() {
            // Create a fake file in the
            var downloader = _services.GetRequiredService<DownloadStorageOptions>();
            string path = downloader.Paths.First();

            if (!File.Exists(Path.Combine(path, MockFileName)))
                File.Create(Path.Combine(path, MockFileName))
                    .Dispose();

            return MockFileName;
        }
    }
}