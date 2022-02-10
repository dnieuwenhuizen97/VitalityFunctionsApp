using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace EmailQueueTrigger
{
    public class EmailQueueTrigger
    {
        private IEmailValidationService ValidationService { get; }

        public EmailQueueTrigger(IEmailValidationService ValidationService)
        {
            this.ValidationService = ValidationService;
        }

        [Function("EmailQueueTrigger")]
        public async Task Run([QueueTrigger("email-verification-queue", Connection = "AzureWebJobsStorage")] string emailQueueItem, FunctionContext context)
        {

            ILogger logger = context.GetLogger("EmailQueueTrigger");

            string[] emailData = emailQueueItem.Split(',');

            await ValidationService.SendValidationEmail(emailData[0], emailData[1]);

            logger.LogInformation($"C# Queue trigger function processed: {emailQueueItem}");
        }

        [Function("RecoveryEmailQueueTrigger")]
        public async Task Recover([QueueTrigger("user-recovery-queue", Connection = "AzureWebJobsStorage")] string queueItem, FunctionContext context)
        {
            string[] data = queueItem.Split(',');

            await ValidationService.SendRecoveryEmail(data[0], data[1]);
        }
    }
}
