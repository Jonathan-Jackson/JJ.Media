using MediaInfo.API.ViewModels;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace MediaInfo.IntegrationTesting.Controller {

    public class SearchControllerEpisodeTest : ControllerTestBase {

        [Theory]
        [InlineData("aaa")]
        [InlineData("123")]
        [InlineData("___")]
        [InlineData("{ }")]
        public async Task NotFound(string fileName) {
            var response = await _client.GetAsync($"/api/search/episode/{fileName}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("Rick & Morty - 05.mkv")]
        [InlineData("South Park - 05.mkv")]
        [InlineData("[PineappleSubs] One Piece - 01 [1080p].mkv")]
        [InlineData("Kitsutsuki Tanteidokoro - 04 [720p].mkv")]
        [InlineData("[--] Princess Connect! Re Dive - 05 [480p].mkv")]
        [InlineData("[WallySubs] A3! Season Spring & Summer - 05 [360p].mkv")]
        public async Task OK(string fileName) {
            var response = await _client.GetAsync($"/api/search/episode/{fileName}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("[PineappleSubs] One Piece - 10001 [1080p].mkv")]
        [InlineData("Rick & Morty - 4353.mkv")]
        public async Task OK_NumberNotReleased(string fileName) {
            var response = await _client.GetAsync($"/api/search/episode/{fileName}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            EpisodeSearchResponse view = JsonSerializer.Deserialize<EpisodeSearchResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.True(view.ShowId > 0);
            Assert.Equal(0, view.Id);
        }
    }
}