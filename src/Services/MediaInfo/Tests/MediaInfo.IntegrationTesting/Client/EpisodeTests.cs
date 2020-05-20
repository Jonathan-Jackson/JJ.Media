using MediaInfo.API.Client.Models;
using System.Threading.Tasks;
using Xunit;

namespace MediaInfo.IntegrationTesting.Client {

    public class EpisodeTests : TestBase {

        [Fact]
        public async Task GetEpisode_OK() {
            Episode episode = await _apiClient.GetEpisode(13);

            Assert.NotNull(episode);
            Assert.True(episode.Id > 0);
        }

        [Fact]
        public async Task GetShowEpisodes_OK() {
            Episode[] episodes = await _apiClient.GetShowEpisodes(6);

            Assert.NotNull(episodes);
            Assert.NotEmpty(episodes);
            Assert.True(episodes[0].ShowId == 6);
        }
    }
}