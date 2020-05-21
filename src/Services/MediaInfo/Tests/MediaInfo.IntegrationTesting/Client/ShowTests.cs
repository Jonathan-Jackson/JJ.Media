using MediaInfo.API.Client.Models;
using System.Threading.Tasks;
using Xunit;

namespace MediaInfo.IntegrationTesting.Client {

    public class ShowTests : TestBase {

        [Fact]
        public async Task GetShow_OK() {
            Show show = await _apiClient.GetShow(13);

            Assert.NotNull(show);
            Assert.NotEmpty(show.Titles);
            Assert.True(show.Id == 13);
        }

        [Fact]
        public async Task GetShowExternalLink_OK() {
            string link = await _apiClient.GetShowExternalLink(13);

            Assert.False(string.IsNullOrWhiteSpace(link));
        }

        [Fact]
        public async Task GetShowImages_OK() {
            string[] images = await _apiClient.GetShowImages(13);

            Assert.NotNull(images);
            Assert.NotEmpty(images);
        }

        [Fact]
        public async Task GetShowOverview_OK() {
            string images = await _apiClient.GetShowOverview(13);

            Assert.False(string.IsNullOrWhiteSpace(images));
        }
    }
}