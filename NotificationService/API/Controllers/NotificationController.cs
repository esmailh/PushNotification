using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Services;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Persistence;
using System;

namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly NotificationDbContext _context;

        public NotificationController(IBackgroundJobClient backgroundJobs, NotificationDbContext context)
        {
            _backgroundJobs = backgroundJobs;
            _context = context;
        }

        /// <summary>
        /// Schedule a new notification to be sent asynchronously via background job.
        /// Tenant ID is taken from X-Tenant-ID header.
        /// </summary>
        [HttpPost]
        public IActionResult SendNotification([FromBody] Notification notification)
        {
            if (HttpContext.Items.TryGetValue("Tenant", out var tenant))
            {
                notification.Tenant = tenant?.ToString();
            }
            else
            {
                notification.Tenant = "default";
            }

            _backgroundJobs.Enqueue<NotificationDispatcher>(d => d.DispatchAsync(notification));
            return Ok("Notification scheduled");
        }

        /// <summary>
        /// Retrieve notification history, optionally filtered by status and date range.
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] NotificationStatus? status = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var query = _context.Notifications.AsQueryable();

            if (HttpContext.Items.TryGetValue("Tenant", out var tenant))
            {
                var Tenant = tenant?.ToString();
                query = query.Where(n => n.Tenant == Tenant);
            }

            if (status.HasValue)
                query = query.Where(n => n.Status == status);

            if (from.HasValue)
                query = query.Where(n => n.CreatedAt >= from);

            if (to.HasValue)
                query = query.Where(n => n.CreatedAt <= to);

            var result = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retry all failed notifications by rescheduling them for delivery.
        /// </summary>
        [HttpPost("retry-failed")]
        public IActionResult RetryFailed()
        {
            _backgroundJobs.Enqueue<NotificationDispatcher>(d => d.RetryFailedAsync());
            return Ok("Retry job scheduled for failed notifications");
        }

        [HttpPost("notification/cancel/{id}")]
        public async Task<IActionResult> CancelNotification(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if ( notification == null )
                return NotFound();

            notification.IsCancelled = true;
            await _context.SaveChangesAsync();

            return Ok("Notification cancelled.");
        }
    }
}
