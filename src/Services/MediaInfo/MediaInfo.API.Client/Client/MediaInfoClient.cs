using JJ.Framework.Client;
using System.Net.Http;

namespace MediaInfo.API.Client.Client {

    public partial class MediaInfoClient : ApiClient {
        private readonly MediaInfoClientOptions _options;

        public MediaInfoClient(HttpClient client, MediaInfoClientOptions options)
            : base(client, options.Address) {
            _options = options;
        }
    }
}