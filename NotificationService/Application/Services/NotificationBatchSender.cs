using NotificationService.Domain.Models;

namespace NotificationService.Application.Services
{
    public class NotificationBatchSender
    {
        private readonly NotificationDispatcher _dispatcher;

        public NotificationBatchSender(NotificationDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task SendBatchAsync(List<string> userIds, NotificationType type, string messageTemplate, Guid campaignId)
        {
            foreach ( var userId in userIds )
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Message = messageTemplate,
                    Type = type,
                    CampaignId = campaignId
                };
                await _dispatcher.DispatchAsync(notification);
            }
        }
    }
}
