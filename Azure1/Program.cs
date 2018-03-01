using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure1
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ngtsample1;AccountKey=yourkey;EndpointSuffix=core.windows.net");
            
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("container");
            CheckContainer(container).Wait();

            container.Metadata.Add("counter", "100");
            container.SetMetadataAsync();

            const string ImageToUpload = @"/Users/macbook/Projects/Azure1/Azure1/kinder.png";
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("kinder.png");
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(ImageToUpload))
            {
                blockBlob.UploadFromStreamAsync(fileStream).Wait();
            }
            GetData(container).Wait();
        }

        private static async Task<List<IListBlobItem>> GetData(CloudBlobContainer container)
        {
            await container.FetchAttributesAsync();
            Console.WriteLine("LastModifiedUTC: " + container.Properties.LastModified);
            Console.WriteLine("ETag: " + container.Properties.ETag);
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await container.ListBlobsSegmentedAsync(continuationToken);
                foreach (var blob in response.Results)
                {
                    Console.WriteLine("- {0} (type: {1})", blob.Uri,
                        blob.StorageUri);
                }
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }

        private static async Task CheckContainer(CloudBlobContainer container)
        {
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
