using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Channels.SmsChannel
{
    public abstract class SmsChannelBase : INotificationChannel
    {
        public abstract Task SendAsync(Notification notification);
    }
}
