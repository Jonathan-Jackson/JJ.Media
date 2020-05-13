using Storage.Domain.Helpers.Events;
using Storage.Domain.Plugins;
using System.Threading.Tasks;
using Xunit;

namespace Storage.IntegrationTesting {

    public class DiscordMessageTests : TestBase {

        [Fact]
        public async Task TryProcessedEpisodeMessage() {
            var discord = _services.GetService(typeof(DiscordPlugin)) as DiscordPlugin;
            var episode = new ProcessedEpisodeEvent {
                EpisodeTitle = "<Test> Episode Title",
                ShowTitle = "<Test> Show Title",
                EpisodeNumber = 1,
                SeasonNumber = 1,
                ShowId = 6,
                EpisodeId = 4
            };

            await Task.Delay(2000); // Discord needs time to connect.
            await discord.TryPromptMessageAsync(episode);
        }
    }
}