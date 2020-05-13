using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Repository;
using Storage.Infrastructure.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Remote {

    public class MediaInfoApi : IMediaInfoRepository {
        private const string SearchEpisodePath = "search/episode";
        private const string ShowImagePath = "show/{0}/images";
        private const string ShowOverviewPath = "show/{0}/overview";
        private const string ShowRemoteLinkPath = "show/{0}/api/showlink";

        private readonly string _address;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<MediaInfoApi> _logger;

        public MediaInfoApi(HttpClient client, ILogger<MediaInfoApi> logger, MediaInfoOptions options, JsonSerializerOptions jsonOptions) {
            _client = client;
            _logger = logger;
            _address = options.Address;
            _jsonOptions = jsonOptions;
        }

        public async Task<string[]> GetShowImages(int showId)
            => await RequestGet<string[]>(string.Format(ShowImagePath, showId));

        public async Task<string> GetShowOverview(int showId)
            => await RequestGet<string>(string.Format(ShowOverviewPath, showId));

        public async Task<string> GetShowRemoteLink(int showId)
            => await RequestGet<string>(string.Format(ShowRemoteLinkPath, showId));

        public async Task<EpisodeSearch> SearchEpisode(string fileName)
            => await RequestGet<EpisodeSearch>($"{SearchEpisodePath}/{fileName}");

        private async Task<TOutput> RequestGet<TOutput>(string subPath, string body = "") {
            string token = Guid.NewGuid().ToString();
            string target = $"{_address}/{subPath}";

            var request = new HttpRequestMessage(HttpMethod.Get, target) {
                Headers = { { "X-Request-ID", token } }
            };

            if (!string.IsNullOrWhiteSpace(body))
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                string json = await response.Content.ReadAsStringAsync();
                return typeof(TOutput) == typeof(string)
                    // this needs improving ~ for some reason CORE returns OK content
                    // as invalid json if it's a string (it needs wrapping in " to be valid).
                    ? (TOutput)Convert.ChangeType(json, typeof(TOutput))
                    : JsonSerializer.Deserialize<TOutput>(json, _jsonOptions);
            }
            else {
                _logger.LogError($"API Request failed. TOKEN: {token} / ADDRESS: {target}");
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}