using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces
{
    public interface INotificationChannel
    {
        Task SendAsync(Notification notification);
    }
}
