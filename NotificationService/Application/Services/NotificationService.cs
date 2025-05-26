using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Interfaces.NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Channels.SmsChannel;
using NotificationService.Infrastructure.Channels;

namespace NotificationService.Application.Services
{
    public class ChannelRegistry : INotificationChannelProvider
    {
        private readonly Dictionary<NotificationType, INotificationChannel> _registry;

        public ChannelRegistry(IEnumerable<INotificationChannel> channels)
        {
            _registry = new Dictionary<NotificationType, INotificationChannel>();

            foreach ( var channel in channels )
            {
                if ( channel is SmsChannelBase )
                    _registry[NotificationType.Sms] = channel;
                else if ( channel is FirebaseChannel )
                    _registry[NotificationType.Push] = channel;
            }
        }

        public INotificationChannel? GetChannel(NotificationType type)
        {
            _registry.TryGetValue(type, out var channel);
            return channel;
        }
    }
}
