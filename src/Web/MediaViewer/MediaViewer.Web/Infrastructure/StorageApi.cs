using MediaViewer.Web.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace MediaViewer.Web.Infrastructure {

    public class StorageApi {
        private const string ProcessedEpisodeOutputByGuid = "api/processedepisodes/guid/{0}/output";

        private readonly string _address;
        private readonly HttpClient _client;
        private readonly ILogger<StorageApi> _logger;

        public StorageApi(HttpClient client, ILogger<StorageApi> logger, StorageApiOptions options) {
            _client = client;
            _logger = logger;
            _address = options.Address;
        }

        public async Task<string> FindEpisodePath(Guid guid) {
            string token = Guid.NewGuid().ToString();
            string target = $"{_address}/{string.Format(ProcessedEpisodeOutputByGuid, HttpUtility.UrlEncode(guid.ToString()))}";

            var request = new HttpRequestMessage(HttpMethod.Get, target) {
                Headers = { { "X-Request-ID", token } }
            };

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                return string.Empty;
        }
    }
}