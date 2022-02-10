using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Interfaces;

namespace EmailQueueTrigger
{
    public class Program
    {
        public static void Main()
        {
            IHost host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(Configure)
                .Build();

            host.Run();
        }

        static void Configure(HostBuilderContext Builder, IServiceCollection Services)
        {
            Services.AddSingleton<IEmailValidationService, EmailValidationService>();
        }
    }
}