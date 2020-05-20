using MediaInfo.API.Client.Models;
using System.Threading.Tasks;
using Xunit;

namespace MediaInfo.IntegrationTesting.Client {

    public class SearchTests : TestBase {

        [Theory]
        [InlineData("Rick & Morty")]
        [InlineData("South Park")]
        [InlineData("One Piece")]
        [InlineData("Kitsutsuki Tanteidokoro")]
        [InlineData("Princess Connect! Re Dive")]
        [InlineData("A3! Season Spring & Summer")]
        [InlineData("Tsugumomo")]
        public async Task ShowSearch_OK(string fileName) {
            int showId = await _apiClient.ShowSearch(fileName);

            Assert.True(showId > 0);
        }

        [Theory]
        [InlineData("[PineappleSubs] One Piece - 10001 [1080p].mkv")]
        [InlineData("Rick & Morty - 4353.mkv")]
        public async Task EpisodeSearch_EpisodeNotFound(string fileName) {
            EpisodeSearchResult episode = await _apiClient.EpisodeSearch(fileName);

            Assert.NotNull(episode);
            Assert.True(episode.Id == 0);
            Assert.True(episode.ShowId > 0);
        }

        [Theory]
        [InlineData("Rick & Morty - 05.mkv")]
        [InlineData("South Park - 05.mkv")]
        [InlineData("[PineappleSubs] One Piece - 01 [1080p].mkv")]
        [InlineData("Kitsutsuki Tanteidokoro - 04 [720p].mkv")]
        [InlineData("[--] Princess Connect! Re Dive - 05 [480p].mkv")]
        [InlineData("[WallySubs] A3! Season Spring & Summer - 05 [360p].mkv")]
        [InlineData("[CheeseTown] Tsugumomo S2 - 05 [1080p].mkv")]
        public async Task EpisodeSearch_OK(string fileName) {
            EpisodeSearchResult episode = await _apiClient.EpisodeSearch(fileName);

            Assert.NotNull(episode);
            Assert.True(episode.Id > 0);
            Assert.True(episode.ShowId > 0);
        }
    }
}