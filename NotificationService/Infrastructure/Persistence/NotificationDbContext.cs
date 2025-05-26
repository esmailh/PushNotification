using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<NotificationCampaign> Campaigns => Set<NotificationCampaign>();
        public DbSet<CampaignJob> CampaignJobs => Set<CampaignJob>();
    }
}
