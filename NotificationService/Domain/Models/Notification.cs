namespace NotificationService.Domain.Models
{
    public enum NotificationType { Sms, Push }
    public enum NotificationStatus { Pending, Sent, Failed }

    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Tenant { get; set; } // Multi-Tenant support
        public string FirebaseToken { get; set; } // For push notifications
        public Guid CampaignId { get; set; }
        public bool IsCancelled { get; set; } = false;
    }
}
