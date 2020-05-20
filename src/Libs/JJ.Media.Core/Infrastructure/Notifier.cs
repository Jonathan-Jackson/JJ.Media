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
        private readonly string _address;

        protected Notifier(ILogger<Notifier<TNotifyType>> logger, string address) {
            _logger = logger;
            _address = string.IsNullOrWhiteSpace(address)
                ? throw new ArgumentException(nameof(address))
                : address;
        }

        public string Address { get; }

        public virtual Task<HttpResponseMessage> Notify(TNotifyType item)
            => Notify(new Notification<TNotifyType>(item));

        public async Task<HttpResponseMessage> TryNotify(Notification<TNotifyType> notification) {
            try {
                return await Notify(notification);
            }
            catch (Exception ex) {
                _logger.LogError($"Failed to notify address: {_address}", ex);
                return null;
            }
        }

        public abstract Task<HttpResponseMessage> Notify(Notification<TNotifyType> notification);

        protected virtual async Task<HttpResponseMessage> PostNotification(Notification<TNotifyType> notification, string body) {
            try {
                var request = new HttpRequestMessage(HttpMethod.Post, _address) {
                    Headers = { { "X-Request-ID", notification.Token.ToString() } },
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };

                return await _client.SendAsync(request);
            }
            catch {
                _logger?.LogError($"Failure notifying: '{_address}' with body: '{body}'");
                throw;
            }
        }
    }
}