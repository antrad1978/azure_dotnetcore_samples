using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorageQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("/Users/macbook/Projects/Azure1/AzureStorageQueue/appsettings.json", true, true)
                .Build();

            var storageAccount =CloudStorageAccount.Parse(config["StorageConnectionString"]);
            var queueClient =  storageAccount.CreateCloudQueueClient();
            
            CloudQueue queue = queueClient.GetQueueReference("queue");
            queue.CreateIfNotExistsAsync().Wait();
            queue.AddMessageAsync(new CloudQueueMessage("Queued message 1"));
            queue.AddMessageAsync(new CloudQueueMessage("Queued message 2"));
            queue.AddMessageAsync(new CloudQueueMessage("Queued message 3"));
            
            ConsumeMessage(queue).Wait();
        }

        private static async Task ConsumeMessage(CloudQueue queue)
        {
            CloudQueueMessage message = await queue.GetMessageAsync();
            if (message != null)
            {
                string messageText = message.AsString;
                System.Console.WriteLine(messageText);
            }
            
            IEnumerable<CloudQueueMessage> batch = await queue.GetMessagesAsync(10);
            foreach (CloudQueueMessage batchMessage in batch)
            {
                Console.WriteLine(batchMessage.AsString);
            }
        }
    }
}