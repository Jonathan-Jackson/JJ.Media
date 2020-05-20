using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JJ.Framework.Client {

    public abstract class ApiClient {
        private readonly HttpClient _client;
        private readonly string _address;

        protected ApiClient(HttpClient client, string address) {
            _client = client;
            _address = address;
        }

        private static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        };

        public Task<HttpResponseMessage> Get(string path)
            => _client.GetAsync(CreateEndAddress(path));

        public async Task<TResult> Get<TResult>(string path) {
            string json = await _client.GetStringAsync(CreateEndAddress(path));
            return JsonSerializer.Deserialize<TResult>(json, JsonOptions);
        }

        public async Task<TResult> Find<TResult>(string path) {
            var response = await _client.GetAsync(CreateEndAddress(path));

            if (response.IsSuccessStatusCode) {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResult>(json, JsonOptions);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
                return default;
            else
                throw new HttpRequestException(response.ReasonPhrase);
        }

        public Task<HttpResponseMessage> Post(string path, object body)
            => Post(path, JsonSerializer.Serialize(body));

        public Task<HttpResponseMessage> Post(string path, string body)
            => _client.PostAsync(CreateEndAddress(path), new StringContent(body, Encoding.UTF8, "application/json"));

        public Task<TResult> Post<TResult>(string path, object body)
            => Post<TResult>(path, JsonSerializer.Serialize(body));

        public async Task<TResult> Post<TResult>(string path, string body) {
            var response = await _client.PostAsync(CreateEndAddress(path), new StringContent(body, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode) {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResult>(json, JsonOptions);
            }
            else
                throw new HttpRequestException(response.ReasonPhrase);
        }

        private string CreateEndAddress(string path)
            => string.IsNullOrEmpty(_address) ? path : $"{_address}/{path}";
    }
}