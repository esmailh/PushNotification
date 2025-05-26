using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces
{
    namespace NotificationService.Domain.Interfaces
    {
        public interface INotificationChannelProvider
        {
            INotificationChannel? GetChannel(NotificationType type);
        }
    }

}
