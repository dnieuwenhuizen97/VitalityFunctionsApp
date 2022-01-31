using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class QueueStorageService : IQueueStorageService
    {
        // TODO : Add file with the configurations for the db connections.

        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private string queueName = "email-verification-queue";
        private CloudStorageAccount storageAccount { get; set; }
        private CloudQueueClient queueClient { get; set; }
        private CloudQueue queue { get; set; }

        public QueueStorageService()
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
            queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(queueName);
        }

        public async Task CreateMessage(string message)
        {
            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            try
            {
                await queue.AddMessageAsync(new CloudQueueMessage(message));
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
