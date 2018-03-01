using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

namespace AzureTables
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("/Users/macbook/Projects/Azure1/AzureTables/appsettings.json", true, true)
                .Build();

            var storageAccount =CloudStorageAccount.Parse(config["StorageConnectionString"]);
            CloudTableClient tableClient =
                storageAccount.CreateCloudTableClient();
            
            CloudTable table = tableClient.GetTableReference("orders");
            //table.DeleteAsync().Wait();
            
            table.CreateIfNotExistsAsync().Wait();

            OrderEntity newOrder = new OrderEntity("Archer", "20141216");
            newOrder.OrderNumber = "101";
            newOrder.ShippedDate = Convert.ToDateTime("12/18/2017");
            
            newOrder.RequiredDate = Convert.ToDateTime("12/14/2017");
            newOrder.Status = "shipped";
            TableOperation insertOperation = TableOperation.Insert(newOrder);
            table.ExecuteAsync(insertOperation).Wait();
            
            TableBatchOperation batchOperation = new TableBatchOperation();
            OrderEntity newOrder1 = new OrderEntity("Lana", "20141217");
            newOrder1.OrderNumber = "102";
            newOrder1.ShippedDate = Convert.ToDateTime("1/1/1900");
            newOrder1.RequiredDate = Convert.ToDateTime("1/1/1900");
            newOrder1.Status = "pending";
            OrderEntity newOrder2 = new OrderEntity("Lana", "20141218");
            newOrder2.OrderNumber = "103";
            newOrder2.ShippedDate = Convert.ToDateTime("1/1/1900");
            newOrder2.RequiredDate = Convert.ToDateTime("12/25/2014");
            newOrder2.Status = "open";
            OrderEntity newOrder3 = new OrderEntity("Lana", "20141219");
            newOrder3.OrderNumber = "103";
            newOrder3.ShippedDate = Convert.ToDateTime("12/17/2014");
            newOrder3.RequiredDate = Convert.ToDateTime("12/17/2014");
            newOrder3.Status = "shipped";
            batchOperation.Insert(newOrder1);
            batchOperation.Insert(newOrder2);
            batchOperation.Insert(newOrder3);
            table.ExecuteBatchAsync(batchOperation).Wait();
            
            //GetOrders(table).Wait();
        }

        private static async Task UpdateData(CloudTable table)
        {
            TableOperation retrieveOperation =
                TableOperation.Retrieve<OrderEntity>("Lana",
                    "20141217");
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);
            OrderEntity updateEntity = (OrderEntity) retrievedResult.Result;
            if (updateEntity != null)
            {
                updateEntity.Status = "shipped";
                updateEntity.ShippedDate = Convert.ToDateTime("12/20/2014");
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);
                table.ExecuteAsync(insertOrReplaceOperation).Wait();
            }

            TableOperation deleteOperation = TableOperation.Delete(updateEntity);
            table.ExecuteAsync(deleteOperation).Wait();
            Console.WriteLine("Entity deleted.");
        }

        private static async Task GetOrders(CloudTable table)
        {
            List<OrderEntity> results=new List<OrderEntity>();
            TableQuery<OrderEntity> query = new TableQuery<OrderEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, "Lana"));
            TableContinuationToken continuationToken = null;
            do
            {
                Task<TableQuerySegment<OrderEntity>> orders = table.ExecuteQuerySegmentedAsync(query, continuationToken);
                List<OrderEntity> temp = orders.Result.ToList<OrderEntity>();
                results.AddRange(temp);
                continuationToken = orders.Result.ContinuationToken;
            } while (continuationToken != null);

            foreach (var order in results)
            {
                System.Console.WriteLine(order.OrderNumber +"\n");
            }

            Console.ReadKey();
        }
    }
}