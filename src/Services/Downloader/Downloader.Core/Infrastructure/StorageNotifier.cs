using JJ.Media.Core.Entities;
using JJ.Media.Core.Infrastructure.Notifier;
using System.Threading.Tasks;

namespace Downloader.Core.Infrastructure {

    public class StorageNotifier : Notifier<string> {
        private string _address;

        public override async Task Notify(Notification<string> notification) {
            await PostNotification(_address, notification, notification.Data);
        }
    }
}