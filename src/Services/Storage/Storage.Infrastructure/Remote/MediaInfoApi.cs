using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Repository;
using Storage.Infrastructure.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Remote {

    public class MediaInfoApi : IMediaInfoRepository {
        private const string SearchEpisodePath = "search/episode";

        private readonly HttpClient _client;
        private readonly string _address;
        private readonly ILogger<MediaInfoApi> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public MediaInfoApi(HttpClient client, ILogger<MediaInfoApi> logger, MediaInfoOptions options, JsonSerializerOptions jsonOptions) {
            _client = client;
            _logger = logger;
            _address = options.Address;
            _jsonOptions = jsonOptions;
        }

        public async Task<EpisodeSearch> SearchEpisode(string fileName) {
            // TODO: Make reusable if required.

            string token = Guid.NewGuid().ToString();
            string target = $"{_address}/{SearchEpisodePath}/{fileName}";

            var request = new HttpRequestMessage(HttpMethod.Get, target) {
                Headers = { { "X-Request-ID", token } }
            };

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EpisodeSearch>(json, _jsonOptions);
            }
            else {
                _logger.LogError($"API Request failed. TOKEN: {token} / ADDRESS: {target}");
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}