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

            var logger = context.GetLogger("EmailQueueTrigger");

            string[] emailData = emailQueueItem.Split(',');

            await ValidationService.SendValidationEmail(emailData[0], emailData[1]);

            logger.LogInformation($"C# Queue trigger function processed: {emailQueueItem}");
        }
    }
}
