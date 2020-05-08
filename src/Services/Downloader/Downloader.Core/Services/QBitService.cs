using Downloader.Core.Helpers;
using Downloader.Core.Helpers.DTOs;
using Downloader.Core.Helpers.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Downloader.Core.Services {

    public class QBitService : ITorrentClient {
        private static HttpClient _client = new HttpClient();

        private readonly ILogger<QBitService> _log;
        private readonly string _address;
        private readonly string _password;
        private readonly string _username;

        // Cookie auth for each request.
        private string _authKey = string.Empty;

        public QBitService(ILogger<QBitService> log, QBitOptions settings) {
            _log = log;
            _address = settings.Address;
            _username = settings.UserName;
            _password = settings.Password;
        }

        /// <summary>
        /// Deletes torrents within QBit Torrent.
        /// </summary>
        /// <param name="hashes">Hash that is assigned to the torrent.</param>
        public async Task DeleteAsync(IEnumerable<string> hashes) {
            HttpRequestMessage request = await CreateDeleteTorrentRequestAsync(string.Join('|', hashes));
            await ProcessQBittorentRequestAsync(request);
        }

        /// <summary>
        /// Starts downloading a torrent within QBit Torrent.
        /// </summary>
        /// <param name="magnet">Magnet URI that references the torrent.</param>
        public async Task DownloadAsync(string magnet) {
            HttpRequestMessage request = await CreateDownloadMagnetRequestAsync(magnet);
            await ProcessQBittorentRequestAsync(request);
        }

        /// <summary>
        /// Returns a collection of completed torrents in QBit Torrent (100% Downloaded).
        /// </summary>
        public async Task<IEnumerable<QBitTorrent>> GetCompletedTorrentsAsync() {
            HttpRequestMessage request = await CreateGetCompletedTorrentsRequestAsync();
            HttpResponseMessage result = await ProcessQBittorentRequestAsync(request);
            return await ReadTorrentResponseAsync(result);
        }

        /// <summary>
        /// Returns a collection of all torrents being processed within QBit Torrent.
        /// </summary>
        public async Task<IEnumerable<QBitTorrent>> GetTorrentsAsync() {
            HttpRequestMessage request = await CreateGetTorrentsRequestAsync();
            HttpResponseMessage result = await ProcessQBittorentRequestAsync(request);
            return await ReadTorrentResponseAsync(result);
        }

        #region Private Methods

        private async Task AddRequestContentAsync(HttpRequestMessage request, string body = "") {
            request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            request.Content.Headers.Add("Content-Length", body.Length.ToString());
            request.Content.Headers.Add("Cookie", await CreateAuthCookieAsync());
        }

        /// <summary>
        /// Creates an authentication cookie to be used by requests.
        /// NOTE: We should do this upon most new requests, as closing
        /// and re-opening QBit will wipe auth tokens. Creating a new
        /// cookie upon each request will prevent breaking if QBit restarts.
        /// </summary>
        private async Task<string> CreateAuthCookieAsync() {
            if (_password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters.");

            HttpRequestMessage request = CreateLoginRequest(_username, _password);
            HttpResponseMessage response = await ProcessQBittorentRequestAsync(request);

            // If the server doesn't reply with an auth key, then
            // it means it's already authenticated.
            _authKey = response.Headers.FirstOrDefault(x => x.Key == "Set-Cookie").Value?.FirstOrDefault()
                        ?? _authKey;
            return _authKey;
        }

        private async Task<HttpRequestMessage> CreateDeleteTorrentRequestAsync(string hashes) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_address}/command/delete");

            string body = $"hashes={HttpUtility.UrlEncode(hashes)}";
            await AddRequestContentAsync(request, body);
            return request;
        }

        private async Task<HttpRequestMessage> CreateDownloadMagnetRequestAsync(string magnet) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_address}/command/download");

            string body = $"urls={HttpUtility.UrlEncode(magnet)}";
            await AddRequestContentAsync(request, body);
            return request;
        }

        private async Task<HttpRequestMessage> CreateGetCompletedTorrentsRequestAsync() {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_address}/query/torrents?filter=completed");
            await AddRequestContentAsync(request);
            return request;
        }

        private async Task<HttpRequestMessage> CreateGetTorrentsRequestAsync() {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_address}/query/torrents");
            await AddRequestContentAsync(request);
            return request;
        }

        private HttpRequestMessage CreateLoginRequest(string username, string password) {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_address}/login");

            string body = $"username={username}&password={password}";
            request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            request.Content.Headers.Add("Content-Length", body.Length.ToString());
            return request;
        }

        private async Task<HttpResponseMessage> ProcessQBittorentRequestAsync(HttpRequestMessage request) {
            try {
                HttpResponseMessage response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode) {
                    throw new HttpRequestException($"An error occured contacting QBitTorrent. The response was: {response.StatusCode} - {response.ReasonPhrase}");
                }

                return response;
            }
            catch {
                _log.LogError($"Unable to contact QBittorrent Web Client. Address used: {_address}");
                throw;
            }
        }

        private async Task<IEnumerable<QBitTorrent>> ReadTorrentResponseAsync(HttpResponseMessage response) {
            string json = await response.Content.ReadAsStringAsync();
            JArray torrents = JArray.Parse(json);

            return torrents.Select(x => new QBitTorrent() {
                Hash = (string)((dynamic)x).hash,
                Name = (string)((dynamic)x).name
            });
        }

        #endregion Private Methods
    }
}