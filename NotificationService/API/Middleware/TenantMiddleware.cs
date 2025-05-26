using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NotificationService.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
            {
                context.Items["TenantId"] = tenantId.ToString();
            }
            else
            {
                context.Items["TenantId"] = "default";
            }

            await _next(context);
        }
    }
}
