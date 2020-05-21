using System.Threading.Tasks;
using Xunit;

namespace Discord.IntegrationTesting.Client {

    public class AlertClientTests : TestBase {

        [Fact]
        public async Task AlertOfEpisode() {
            await _apiClient.AlertOfEpisode(127);
        }
    }
}