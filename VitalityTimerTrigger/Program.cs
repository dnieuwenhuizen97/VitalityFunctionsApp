using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Infrastructure.StorageAccount;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Security;
using Security.Interfaces;
using Services;
using Services.Interfaces;
using System.Threading.Tasks;

namespace VitalityTimerTrigger
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(Configure)
                .Build();

            host.Run();
        }
        static void Configure(HostBuilderContext Builder, IServiceCollection Services)
        {
			Services.AddSingleton<IJwtTokenService, JwtTokenService>();
			Services.AddSingleton<IRequestValidator, RequestValidator>();
			Services.AddSingleton<IEmailValidationService, EmailValidationService>();
			Services.AddSingleton<ITimelineService, TimelineService>();
			Services.AddSingleton<IUserService, UserService>();
			Services.AddTransient<INotificationService, NotificationService>();
			Services.AddScoped<IBlobStorage, BlobStorage>();
			Services.AddScoped<IBlobStorageService, BlobStorageService>();
			Services.AddScoped<IChallengeService, ChallengeService>();
			Services.AddSingleton<IQueueStorageService, QueueStorageService>();
			Services.AddSingleton<IUserDb, UserDb>();
			Services.AddScoped<IChallengeDb, ChallengeDb>();
			Services.AddScoped<IPushTokenDb, PushTokenDb>();
			Services.AddScoped<INotificationDb, NotificationDb>();
			Services.AddScoped<ITimelineDb, TimelineDb>();
			Services.AddScoped<ILikeDb, LikeDb>();
			Services.AddScoped<ICommentDb, CommentDb>();
			Services.AddSingleton<DbContextDomains>();
			Services.AddSingleton<IInputSanitizationService, InputSanitizationService>();
		}
    }
}