namespace NotificationService.API.Controllers
{
    using Hangfire;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using NotificationService.Application.Services;
    using NotificationService.Domain.Models;
    using NotificationService.Infrastructure.Options;
    using NotificationService.Infrastructure.Persistence;

    [ApiController]
    [Route("api/campaigns")]
    public class NotificationCampaignController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly NotificationDbContext _context;
        private readonly IOptions<BatchOptions> _batchOptions;
        private readonly JobTrackingService _jobTracker;

        public NotificationCampaignController(
       IBackgroundJobClient backgroundJobs,
       NotificationDbContext context,
       IOptions<BatchOptions> batchOptions,
       JobTrackingService jobTracker)
        {
            _backgroundJobs = backgroundJobs;
            _context = context;
            _batchOptions = batchOptions;
            _jobTracker = jobTracker;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartCampaign([FromBody] NotificationCampaign campaign, [FromQuery] List<string> userIds)
        {
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();

            int batchSize = _batchOptions.Value.Size;
            int total = userIds.Count;
            int batches = (int) Math.Ceiling((double) total / batchSize);

            for ( int i = 0; i < batches; i++ )
            {
                var batch = userIds.Skip(i * batchSize).Take(batchSize).ToList();
                var jobId = _backgroundJobs.Enqueue<NotificationBatchSender>(s =>
                    s.SendBatchAsync(batch, campaign.Type, campaign.MessageTemplate, campaign.Id));

                await _jobTracker.TrackJobAsync(campaign.Id, jobId);
            }

            return Ok(new { campaign.Id, message = "Campaign started.", jobId });
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelCampaign(Guid id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if ( campaign == null )
                return NotFound();

            campaign.IsCancelled = true;
            await _context.SaveChangesAsync();

            await _jobTracker.CancelJobsForCampaignAsync(id);

            return Ok("Campaign cancelled.");
        }
    }
}
