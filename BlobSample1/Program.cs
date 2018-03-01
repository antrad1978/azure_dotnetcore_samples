using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobSample1
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount secureStorageAccount = new CloudStorageAccount(new
                StorageCredentials("ngtsample1", "yourkey"), true);
            System.Console.WriteLine(secureStorageAccount.Credentials.SASSignature);
                
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ngtsample1;AccountKey=yourkey;EndpointSuffix=core.windows.net");
            
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("container");
            
            string pageBlobName = "random";
            ManageBlob(container, pageBlobName).Wait();
            
            var sample=new CustomClass();
            sample.Id = 1;
            sample.Name = "Test";
            SaveObjectInBlob<CustomClass>(container, pageBlobName, sample).Wait();
        }

        private static async Task ManageBlob(CloudBlobContainer container, string pageBlobName)
        {
            CloudPageBlob pageBlob = container.GetPageBlobReference(pageBlobName);
            await pageBlob.CreateAsync(512 * 2 /*size*/); // size needs to be multiple of 512 bytes
            byte[] samplePagedata = new byte[512];
            Random random = new Random();
            random.NextBytes(samplePagedata);
            await pageBlob.UploadFromByteArrayAsync(samplePagedata, 0, samplePagedata.Length);

            int bytesRead = await
                pageBlob.DownloadRangeToByteArrayAsync(samplePagedata,
                    0, 0, samplePagedata.Length);
            System.Console.WriteLine("Read:" + bytesRead +" bytes");
        }
        
        private static async Task SaveObjectInBlob<T>(CloudBlobContainer container, string pageBlobName,T obj)
        {
            CloudPageBlob pageBlob = container.GetPageBlobReference(pageBlobName);
            await pageBlob.CreateAsync(512 * 2 /*size*/); // size needs to be multiple of 512 bytes
            byte[] samplePagedata = new byte[2048];
            var serializedObject = ObjectToByteArray<T>(obj);
            serializedObject.CopyTo(samplePagedata, 0);
            await pageBlob.UploadFromByteArrayAsync(samplePagedata, 0, samplePagedata.Length);

            int bytesRead = await
                pageBlob.DownloadRangeToByteArrayAsync(samplePagedata,
                    0, 0, samplePagedata.Length);
            System.Console.WriteLine("Read:" + bytesRead +" bytes");
        }
        
        private static byte[] ObjectToByteArray<T>(T obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        
        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}