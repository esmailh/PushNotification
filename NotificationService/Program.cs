using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Middleware;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Interfaces.NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Channels;
using NotificationService.Infrastructure.Channels.SmsChannel;
using NotificationService.Infrastructure.Options;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.ServiceExtentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseInMemoryDatabase("NotificationDb"));

builder.Services.AddScoped<NotificationDispatcher>();
builder.Services.AddScoped<INotificationChannel, KavenegarSmsChannel>();
builder.Services.AddScoped<INotificationChannel, FirebaseChannel>();

builder.Services.AddLogging();
builder.Services.RegisterHangfireDependencies(builder.Configuration);
builder.Services.AddHangfireServer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterFirebaseDependencies(builder.Configuration);
builder.Services.Configure<BatchOptions>(builder.Configuration.GetSection("BatchOptions"));

builder.Services.AddScoped<INotificationChannel, KavenegarSmsChannel>();
builder.Services.AddScoped<INotificationChannel, FirebaseChannel>();
builder.Services.AddScoped<JobTrackingService>();

builder.Services.AddSingleton<INotificationChannelProvider>(provider =>
{
    var channels = provider.GetServices<INotificationChannel>();
    return new ChannelRegistry(channels);
});


var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();

app.UseRouting();
app.UseAuthorization();

app.UseHangfireDashboard("/pushnotification/hangfire", new DashboardOptions
{
    Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = "admin",
                    Pass = "0Kala"
                }
            }
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

await app.RunAsync();
