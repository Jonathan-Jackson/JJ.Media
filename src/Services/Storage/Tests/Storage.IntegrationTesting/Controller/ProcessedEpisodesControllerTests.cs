using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Storage.IntegrationTesting {

    public class ProcessedEpisodesControllerTests : TestBase {

        [Fact]
        public async Task GetEpisodeGuids() {
            var ids = Enumerable.Range(1, 200).ToArray();
            var response = await _client.PostAsync("api/processedepisodes/guid/episode", new StringContent(JsonSerializer.Serialize(ids), Encoding.UTF8, "application/json"));
            Assert.True(response.IsSuccessStatusCode);

            string json = await response.Content.ReadAsStringAsync() ?? "";
            Assert.True(json.Any());

            var result = JsonSerializer.Deserialize<dynamic[]>(json);
            Assert.NotEmpty(result);
        }
    }
}