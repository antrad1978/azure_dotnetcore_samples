using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace SAS
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ngtsample1;AccountKey=yourkey;EndpointSuffix=core.windows.net");
            
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            SharedAccessBlobPolicy sasPolicy = new SharedAccessBlobPolicy();
            sasPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1);
            sasPolicy.SharedAccessStartTime = DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0));
            sasPolicy.Permissions = SharedAccessBlobPermissions.Read |
                                    SharedAccessBlobPermissions.
                                        Write | SharedAccessBlobPermissions.Delete |
                                    SharedAccessBlobPermissions.List;
            CloudBlobContainer files = blobClient.GetContainerReference("files");
            string sasContainerToken = files.GetSharedAccessSignature(sasPolicy);
            System.Console.WriteLine(sasContainerToken);
            
            StorageCredentials creds = new StorageCredentials(sasContainerToken);
            CloudStorageAccount accountWithSAS = new
                CloudStorageAccount(creds, "ngtsample1", endpointSuffix: null, useHttps: true);
            //CloudBlobClientCloudBlobContainer sasFiles = sasClient.GetContainerReference("files");
        }

        private static void ManageQueueAccess(CloudStorageAccount account)
        {
            CloudQueueClient queueClient = account.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("queue");
            SharedAccessQueuePolicy sasPolicy = new SharedAccessQueuePolicy();
            sasPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1);
            sasPolicy.Permissions = SharedAccessQueuePermissions.Read |
                                    SharedAccessQueuePermissions.Add |
                                    SharedAccessQueuePermissions.Update |
                                    SharedAccessQueuePermissions.ProcessMessages;
            sasPolicy.SharedAccessStartTime = DateTime.UtcNow.Subtract(new
                TimeSpan(0, 5, 0));
            string sasToken = queue.GetSharedAccessSignature(sasPolicy);
            
            StorageCredentials creds = new StorageCredentials(sasToken);
            CloudQueueClient sasClient = new CloudQueueClient(new
                Uri("https://dataike1.queue.core.windows.net/"), creds);
            CloudQueue sasQueue = sasClient.GetQueueReference("queue");
            sasQueue.AddMessageAsync(new CloudQueueMessage("new message")).Wait();
            Console.ReadKey();
        }

        private static void ManageTablesAccess(CloudStorageAccount account)
        {
            CloudTableClient tableClient =
                account.CreateCloudTableClient();
            
            CloudTable table = tableClient.GetTableReference("orders");
            SharedAccessTablePolicy sasPolicy = new SharedAccessTablePolicy();
            sasPolicy.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1);
            sasPolicy.Permissions = SharedAccessTablePermissions.Query |
                                    SharedAccessTablePermissions.Add |
                                    SharedAccessTablePermissions.Update |
                                    SharedAccessTablePermissions.Delete;
            sasPolicy.SharedAccessStartTime = DateTime.UtcNow.Subtract(new
                TimeSpan(0, 5, 0));
            string sasToken = table.GetSharedAccessSignature(sasPolicy);
        }
    }
}