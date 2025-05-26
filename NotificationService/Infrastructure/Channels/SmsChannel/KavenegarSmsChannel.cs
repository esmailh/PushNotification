using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Channels.SmsChannel
{
    public class KavenegarSmsChannel : SmsChannelBase
    {
        public override Task SendAsync(Notification notification)
        {
            Console.WriteLine($"[Kavenegar] SMS to {notification.UserId} (Tenant: {notification.TenantId}): {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
