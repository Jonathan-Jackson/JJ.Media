using Downloader.Core.Helpers.Options;
using JJ.Media.Core.Entities;
using JJ.Media.Core.Infrastructure.Notifier;
using System.Threading.Tasks;

namespace Downloader.Core.Infrastructure {

    public class StorageServiceNotifier : Notifier<string> {
        private string _address;

        public StorageServiceNotifier(StorageServiceOptions options) {
            _address = options.Address;
        }

        public override async Task Notify(Notification<string> notification) {
            await PostNotification($"{_address}/media/process", notification, notification.Data);
        }
    }
}