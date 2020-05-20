using JJ.Framework.Client;
using System.Net.Http;

namespace Converter.API.Client.Client {

    public partial class ConverterClient : ApiClient {
        private readonly ConverterClientOptions _options;

        public ConverterClient(HttpClient client, ConverterClientOptions options)
            : base(client, options.Address) {
            _options = options;
        }
    }
}