using Downloader.Core.Helpers.Options;
using JJ.Media.Core.Entities;
using JJ.Media.Core.Infrastructure.Notification;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Downloader.Core.Infrastructure {

    public class StorageServiceNotifier : Notifier<string> {
        private string _address;

        public StorageServiceNotifier(StorageServiceOptions options, ILogger<Notifier<string>> logger)
                : base(logger) {
            _address = options.Address;
        }

        public override async Task<HttpResponseMessage> Notify(Notification<string> notification)
           => await PostNotification($"{_address}media/process", notification, $"\"{notification.Data}\"");
    }
}