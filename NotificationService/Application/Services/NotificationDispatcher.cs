using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Interfaces.NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace NotificationService.Application.Services
{
    public class NotificationDispatcher
    {
        private readonly IEnumerable<INotificationChannel> _channels;
        private readonly ILogger<NotificationDispatcher> _logger;
        private readonly NotificationDbContext _context;
        private readonly INotificationChannelProvider _channelProvider;

        // Rate limiting in-memory store: Tenant -> last send time
        private static readonly ConcurrentDictionary<string, DateTime> _lastSentTimes = new();

        // Minimal interval between notifications per tenant
        private readonly TimeSpan _minInterval = TimeSpan.FromSeconds(1);

        public NotificationDispatcher(IEnumerable<INotificationChannel> channels,
            ILogger<NotificationDispatcher> logger,
            NotificationDbContext context,
            INotificationChannelProvider channelProvider)
        {
            _channels = channels;
            _logger = logger;
            _context = context;
            _channelProvider = channelProvider;
        }

        public async Task DispatchAsync(Notification notification)
        {

            if ( notification.IsCancelled )
            {
                _logger.LogWarning("Notification {NotificationId} is cancelled. Skipping.", notification.Id);
                return;
            }

            //var campaign = await _context.Campaigns.FindAsync(notification.CampaignId);
            //if ( campaign?.IsCancelled == true )
            //{
            //    _logger.LogWarning("Campaign {CampaignId} is cancelled. Skipping notification.", notification.CampaignId);
            //    return;
            //}

            if ( IsRateLimited(notification.Tenant) )
            {
                notification.Status = NotificationStatus.Failed;
                notification.ErrorMessage = "Rate limit exceeded.";
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                _logger.LogWarning("Rate limit exceeded for tenant {Tenant}", notification.Tenant);
                return;
            }

            var channel = _channelProvider.GetChannel(notification.Type);

            if ( channel == null )
            {
                notification.Status = NotificationStatus.Failed;
                notification.ErrorMessage = $"No suitable channel found for {notification.Type}";
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                _logger.LogError("No suitable channel found for notification type: {Type}", notification.Type);
                return;
            }

            try
            {
                await channel.SendAsync(notification);
                notification.Status = NotificationStatus.Sent;
                _logger.LogInformation("Notification sent via {Channel} for user {UserId} tenant {Tenant}",
                    channel.GetType().Name, notification.UserId, notification.Tenant);

                _lastSentTimes[notification.Tenant] = DateTime.UtcNow;
            }
            catch ( Exception ex )
            {
                notification.Status = NotificationStatus.Failed;
                notification.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Failed to send notification for user {UserId}", notification.UserId);
            }

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        private bool IsRateLimited(string Tenant)
        {
            if ( _lastSentTimes.TryGetValue(Tenant, out var lastSent) )
            {
                return (DateTime.UtcNow - lastSent) < _minInterval;
            }
            return false;
        }

        public async Task RetryFailedAsync()
        {
            var failedNotifications = _context.Notifications
                .Where(n => n.Status == NotificationStatus.Failed && !n.IsCancelled)
                .ToList();

            foreach ( var notification in failedNotifications )
            {
                notification.Status = NotificationStatus.Pending;
                notification.ErrorMessage = null;
                await DispatchAsync(notification);
            }

            await _context.SaveChangesAsync();
        }

    }
}
