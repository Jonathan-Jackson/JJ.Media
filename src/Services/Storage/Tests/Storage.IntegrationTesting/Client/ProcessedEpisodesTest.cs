using System;
using System.Threading.Tasks;
using Xunit;

namespace Storage.IntegrationTesting.Client {

    public class ProcessedEpisodesTest : TestBase {

        [Fact]
        public async Task GuidByEpisode_OK() {
            Guid guid = await _apiClient.GetGuidByEpisode(128);

            Assert.NotEqual(Guid.Empty, guid);
        }

        [Fact]
        public async Task GuidByEpisodes_OK() {
            var guids = await _apiClient.GetGuidByEpisode(new[] { 125, 126, 127 });

            Assert.Contains(guids, ep => ep.EpisodeId == 125);
        }

        [Fact]
        public async Task OutputByGuid_OK() {
            string output = await _apiClient.GetOutputByGuid(Guid.Parse("621B7399-87D3-4905-B99B-F70BA2C1725A"));

            Assert.False(string.IsNullOrWhiteSpace(output));
        }

        [Fact]
        public async Task OutputByEpisode() {
            string output = await _apiClient.GetOutputByEpisode(125);

            Assert.False(string.IsNullOrWhiteSpace(output));
        }
    }
}