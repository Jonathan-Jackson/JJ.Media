using JJ.Media.Core.Entities;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Media.Core.Infrastructure.Notifier {

    public abstract class Notifier<TNotifyType> {
        protected static HttpClient _client = new HttpClient();

        public abstract Task Notify(Notification<TNotifyType> notification);

        protected virtual async Task PostNotification(string address, Notification<TNotifyType> notification, string body) {
            var request = new HttpRequestMessage(HttpMethod.Post, address) {
                Headers = { { "X-Request-ID", notification.Token.ToString() } },
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            await _client.SendAsync(request);
        }
    }
}