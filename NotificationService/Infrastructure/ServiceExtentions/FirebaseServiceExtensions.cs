using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using NotificationService.Infrastructure.ServiceExtentions.Settings;

namespace NotificationService.Infrastructure.ServiceExtentions
{
    public static class FirebaseServiceExtensions
    {
        public static void RegisterFirebaseDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            var firebaseSettings = configuration.GetSection("FirebaseSettings").Get<FirebaseSettings>();

            FirebaseApp appInstance = FirebaseApp.DefaultInstance ?? FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(firebaseSettings.Credentials),
            });

            services.AddSingleton(appInstance);
        }
    }
}
