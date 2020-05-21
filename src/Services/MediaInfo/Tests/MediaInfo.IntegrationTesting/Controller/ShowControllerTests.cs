using MediaInfo.Domain.Helpers.DTOs.Shows;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace MediaInfo.IntegrationTesting.Controller {

    public class ShowControllerTests : TestBase {

        [Theory]
        [InlineData(6)]
        public async Task OK(int showId) {
            var response = await _client.GetAsync($"/api/show/{showId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string data = await response.Content.ReadAsStringAsync();
            var show = JsonSerializer.Deserialize<Show>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.Equal(showId, show.Id);
            Assert.NotEmpty(show.Titles);
        }

        [Theory]
        [InlineData("999999999")]
        public async Task NotFound(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("0")]
        public async Task BadRequest(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("6")]
        public async Task Overview_OK(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/overview");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string data = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(data));
        }

        [Theory]
        [InlineData("999999999")]
        public async Task Overview_NotFound(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/overview");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("0")]
        public async Task Overview_BadRequest(string showId) {
            var response = await _client.GetAsync($"/api/show/{showId}/overview");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

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