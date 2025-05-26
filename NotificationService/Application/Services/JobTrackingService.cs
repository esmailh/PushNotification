// Application/Services/JobTrackingService.cs
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Persistence;

public class JobTrackingService
{
    private readonly NotificationDbContext _context;

    public JobTrackingService(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task TrackJobAsync(Guid campaignId, string jobId)
    {
        _context.CampaignJobs.Add(new CampaignJob
        {
            CampaignId = campaignId,
            JobId = jobId
        });
        await _context.SaveChangesAsync();
    }

    public async Task CancelJobsForCampaignAsync(Guid campaignId)
    {
        var jobs = await _context.CampaignJobs
       .Where(j => j.CampaignId == campaignId && !j.IsCancelled)
       .ToListAsync();

        foreach ( var job in jobs )
        {
            BackgroundJob.Delete(job.JobId);
            job.IsCancelled = true;
        }

        await _context.SaveChangesAsync();
    }

}
