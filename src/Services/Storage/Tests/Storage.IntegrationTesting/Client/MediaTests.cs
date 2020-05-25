using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Storage.IntegrationTesting.Client {

    public class MediaTests : TestBase {

        [Fact]
        public async Task ProcessAnime_BadRequest() {
            var response = await _apiClient.ProcessAnime("______");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}