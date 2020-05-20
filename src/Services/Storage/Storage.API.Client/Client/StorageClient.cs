using JJ.Framework.Client;
using System.Net.Http;

namespace Storage.API.Client.Client {

    public partial class StorageClient : ApiClient {
        private readonly StorageClientOptions _options;

        public StorageClient(HttpClient client, StorageClientOptions options)
            : base(client, options.Address) {
            _options = options;
        }
    }
}