using Downloader.Core.Helpers;
using Downloader.Core.Helpers.DTOs;
using Downloader.Core.Helpers.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Downloader.Core.Services {

    public class QBitService : ITorrentClient {
        private static HttpClient _client = new HttpClient();
        private static string _lastSid;

        private readonly ILogger<QBitService> _log;
        private readonly string _address;
        private readonly string _password;
        private readonly string _username;

        public QBitService(ILogger<QBitService> log, QBitOptions settings) {
            _log = log;
            _address = settings.Address;
            _username = settings.UserName;
            _password = settings.Password;
        }

        public async Task<bool> TryAuth()
            => !string.IsNullOrWhiteSpace(await GetCookieAuthAsync());

        /// <summary>
        /// Deletes torrents within QBit Torrent.
        /// </summary>
        /// <param name="hashes">Hash that is assigned to the torrent.</param>
        public Task TryDeleteAsync(IEnumerable<string> hashes) {
            return Task.WhenAll(hashes.Select(async hash => {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"{_address}/api/v2/torrents/delete?hashes={hash}&deleteFiles=false")) {
                    request.Headers.TryAddWithoutValidation("Cookie", await GetCookieAuthAsync());
                    await _client.SendAsync(request);
                }
            }));
        }

        /// <summary>
        /// Starts downloading a torrent within QBit Torrent.
        /// </summary>
        /// <param name="magnet">Magnet URI that references the torrent.</param>
        public async Task DownloadAsync(string magnet) {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{_address}/api/v2/torrents/add")) {
                request.Headers.TryAddWithoutValidation("Cookie", await GetCookieAuthAsync());

                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent(magnet), "urls");

                request.Content = multipartContent;

                var response = await _client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationException($"Failed to authorize! Username: {_username}, Password: {_password}");
                else if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to get torrents on {_address}, response: {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// Returns a collection of completed torrents in QBit Torrent (100% Downloaded).
        /// </summary>
        public Task<IEnumerable<QBitTorrent>> GetCompletedTorrentsAsync()
            => GetFilteredTorrents("?filter=completed");

        /// <summary>
        /// Returns a collection of all torrents being processed within QBit Torrent.
        /// </summary>
        public Task<IEnumerable<QBitTorrent>> GetTorrentsAsync()
            => GetFilteredTorrents();

        private async Task<IEnumerable<QBitTorrent>> GetFilteredTorrents(string filter = "") {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"{_address}/api/v2/torrents/info{filter}")) {
                request.Headers.TryAddWithoutValidation("Cookie", await GetCookieAuthAsync());

                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return await ReadTorrentResponseAsync(response);
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AuthenticationException($"Failed to authorize! Username: {_username}, Password: {_password}");
                else
                    throw new HttpRequestException($"Failed to get torrents on {_address}, response: {response.ReasonPhrase}");
            }
        }

        #region Private Methods

        /// <summary>
        /// Creates an authentication cookie to be used by requests.
        /// NOTE: We should do this upon most new requests, as closing
        /// and re-opening QBit will wipe auth tokens. Creating a new
        /// cookie upon each request will prevent breaking if QBit restarts.
        /// </summary>
        private async Task<string> GetCookieAuthAsync() {
            using (var authRequest = new HttpRequestMessage(HttpMethod.Post, $"{_address}/api/v2/auth/login")) {
                var body = $"username={_username}&password={_password}";
                authRequest.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                authRequest.Content.Headers.Add("Content-Length", body.Length.ToString());
                authRequest.Headers.Add("Referer", _address);

                var response = await _client.SendAsync(authRequest);
                var sid = response.Headers.FirstOrDefault(x => string.Equals(x.Key, "Set-Cookie", StringComparison.OrdinalIgnoreCase))
                                            .Value?.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(sid))
                    _lastSid = sid;

                return !string.IsNullOrWhiteSpace(_lastSid)
                    ? _lastSid
                    : throw new AuthenticationException($"Could not authenticate with QBitTorrent at {_address}. (HTTP Response: {response.ReasonPhrase})");
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