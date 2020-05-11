using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace MediaInfo.IntegrationTesting.Controller {

    public class ShowControllerTests : TestBase {

        [Theory]
        [InlineData("6")]
        public async Task ApiShowLink_OK(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/api/showlink");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string data = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(data));
        }

        [Theory]
        [InlineData("999999999")]
        public async Task ApiShowLink_NotFound(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/api/showlink");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("0")]
        public async Task ApiShowLink_BadRequest(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/api/showlink");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("6")]
        public async Task Images_OK(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/images");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string data = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(data));
        }

        [Theory]
        [InlineData("9999999")]
        public async Task Images_NotFound(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/images");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Images_BadRequest() {
            var response = await _client.GetAsync($"/api/show/{0}/images");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}