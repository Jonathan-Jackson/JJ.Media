using JJ.Framework.Client;
using System.Net.Http;

namespace Discord.API.Client.Client {

    public partial class DiscordClient : ApiClient {

        public DiscordClient(HttpClient client, DiscordClientOptions options)
            : base(client, options.Address) {
        }
    }
}