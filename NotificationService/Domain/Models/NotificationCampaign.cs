namespace NotificationService.Domain.Models
{
    public class NotificationCampaign
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public NotificationType Type { get; set; }
        public string MessageTemplate { get; set; }
        public bool IsCancelled { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ScheduledAt { get; set; }

    }
}
