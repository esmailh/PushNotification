using Hangfire;
using Hangfire.Redis.StackExchange;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NotificationService.Infrastructure.ServiceExtentions
{
    public static class HangfireExtensions
    {
        public static void RegisterHangfireDependencies(this IServiceCollection services,
        IConfiguration configuration)
        {
            var options = new RedisStorageOptions()
            {
                Prefix = configuration.GetSection("Hangfire:Prefix").Get<string>(),
                Db = configuration.GetSection("Hangfire:DBNumber").Get<int>(),
                InvisibilityTimeout = TimeSpan.FromSeconds(60),
            };

#if DEBUG
            string redisConfig = configuration.GetSection("Redis:ConnectionStringLocal").Get<string>();
#elif STAGE
            string redisConfig = configuration.GetSection("Redis:ConnectionString").Get<string>();
#else
            string redisConfig = configuration.GetSection("Redis:ConnectionString").Get<string>();
#endif

            var redis = ConnectionMultiplexer.Connect(redisConfig);

            services.AddHangfire(configuration =>
            {
                configuration
                    .UseRedisStorage(redis, options)
                    .WithJobExpirationTimeout(TimeSpan.FromMinutes(60))
                    .UseSerializerSettings(new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        TypeNameHandling = TypeNameHandling.Auto
                    })
                    .UseSimpleAssemblyNameTypeSerializer();
            });

            services.AddHangfireServer(backgroundJobServerOptions =>
            {
                backgroundJobServerOptions.WorkerCount = 3;
                backgroundJobServerOptions.Queues = new[]
                {
                "push","multicast-push"
                };
                backgroundJobServerOptions.CancellationCheckInterval = TimeSpan.FromSeconds(5);
            });
        }
    }
}
