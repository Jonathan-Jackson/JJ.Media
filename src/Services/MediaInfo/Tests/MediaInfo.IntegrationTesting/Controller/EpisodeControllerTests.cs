using MediaInfo.Domain.Helpers.DTOs.Episodes;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace MediaInfo.IntegrationTesting.Controller {

    public class EpisodeControllerTests : TestBase {

        [Theory]
        [InlineData("999999999")]
        public async Task NotFound(string episodeId) {
            var response = await _client.GetAsync($"/api/episode/{episodeId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("aaa")]
        [InlineData("-1")]
        public async Task BadRequest(string episodeId) {
            var response = await _client.GetAsync($"/api/episode/{episodeId}");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(6)]
        public async Task OK(int episodeId) {
            var response = await _client.GetAsync($"/api/episode/{episodeId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            var episode = JsonSerializer.Deserialize<Episode>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(episodeId, episode.Id);
        }

        [Theory]
        [InlineData("aaa")]
        [InlineData("-1")]
        public async Task ShowEpisodes_BadRequest(string showId) {
            var response = await _client.GetAsync($"/api/episode/show/{showId}");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(6)]
        public async Task ShowEpisodes_OK(int showId) {
            var response = await _client.GetAsync($"/api/episode/show/{showId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            var episodes = JsonSerializer.Deserialize<Episode[]>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.True(episodes.Any());
            Assert.True(episodes[0].Id > 0);
        }
    }
}