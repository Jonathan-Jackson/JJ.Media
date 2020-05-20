using JJ.Framework.Client;
using System.Threading.Tasks;

namespace Discord.API.Client.Client {

    public partial class DiscordClient : ApiClient {

        public Task AlertOfEpisode(int episodeId)
            => Post("/api/alert/episode", episodeId.ToString());
    }
}