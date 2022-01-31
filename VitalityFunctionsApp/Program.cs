using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Infrastructure.StorageAccount;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Functions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Security;
using Security.Interfaces;
using Services;
using Services.Interfaces;

namespace VitalityFunctionsApp
{
	public class Program
	{
		public static void Main()
		{
			IHost host = new HostBuilder()
				.ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson().UseMiddleware<JwtMiddleware>())
				.ConfigureServices(Configure)
				.Build();

			host.Run();
		}

		static void Configure(HostBuilderContext Builder, IServiceCollection Services)
		{
			Services.AddSingleton<IOpenApiHttpTriggerContext, OpenApiHttpTriggerContext>();
			Services.AddSingleton<IOpenApiTriggerFunction, OpenApiTriggerFunction>();
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