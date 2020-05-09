using JJ.Media.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Media.Core.Infrastructure.Notification {

    public abstract class Notifier<TNotifyType> {
        protected static HttpClient _client = new HttpClient();
        protected ILogger<Notifier<TNotifyType>> _logger;

        protected Notifier(ILogger<Notifier<TNotifyType>> logger) {
            _logger = logger;
        }

        public abstract Task<HttpResponseMessage> Notify(Notification<TNotifyType> notification);

        protected virtual async Task<HttpResponseMessage> PostNotification(string address, Notification<TNotifyType> notification, string body) {
            try {
                var request = new HttpRequestMessage(HttpMethod.Post, address) {
                    Headers = { { "X-Request-ID", notification.Token.ToString() } },
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };

                return await _client.SendAsync(request);
            }
            catch {
                _logger?.LogError($"Failure notifying: '{address}' with body: '{body}'");
                throw;
            }
        }
    }
}