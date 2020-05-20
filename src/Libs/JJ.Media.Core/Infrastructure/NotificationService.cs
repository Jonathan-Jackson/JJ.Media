using JJ.Media.Core.Entities;
using JJ.Media.Core.Infrastructure.Notification;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JJ.Media.Core.Infrastructure {

    public class NotificationService<TNotifyObject> {
        private readonly Notifier<TNotifyObject>[] _notifers;
        private readonly ILogger<NotificationService<TNotifyObject>> _logger;

        public NotificationService(Notifier<TNotifyObject>[] notifers) {
            _notifers = notifers;
        }

        public NotificationService(Notifier<TNotifyObject>[] notifers, ILogger<NotificationService<TNotifyObject>> logger)
            : this(notifers) {
            _logger = logger;
        }

        public async Task TrySendNotifications(TNotifyObject episode) {
            var notification = new Notification<TNotifyObject>(episode);

            _logger?.LogDebug($"Sending notifications for: {notification.Token}");

            foreach (var reciever in _notifers)
                await reciever.TryNotify(notification);
        }

        public async Task SendNotifications(TNotifyObject episode) {
            var notification = new Notification<TNotifyObject>(episode);

            _logger?.LogDebug($"Sending notifications for: {notification.Token}");

            foreach (var reciever in _notifers)
                await reciever.Notify(notification);
        }
    }
}