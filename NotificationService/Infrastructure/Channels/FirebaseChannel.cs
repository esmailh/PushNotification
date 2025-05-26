using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Channels
{
    public class FirebaseChannel : INotificationChannel
    {
        public Task SendAsync(Notification notification)
        {
            Console.WriteLine($"[Firebase] PUSH to {notification.UserId} (Tenant: {notification.TenantId}): {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
