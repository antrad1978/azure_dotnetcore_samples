using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Azure.DocumentDb.Sample
{
    class Program
    {
        private const string EndpointUrl = "https://ngtcosmos1.documents.azure.com:443/";
        private const string PrimaryKey = "KV7aLrAxbja4CeWtc4NbmmjXwkUpQVhDE9n2BNjpwYP7G20Pq9WGlE7u1p1egIIbVdvjnGRrTNDTbSql9ajPxA==";
        private static DocumentClient client;
        
        static void Main(string[] args)
        {
            GetStartedDemo().Wait();
        }
        
        // ADD THIS PART TO YOUR CODE
        private static async Task GetStartedDemo()
        {
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            string id = "SalesDB";
            var database = client.CreateDatabaseQuery().AsEnumerable().Where(db => db.Id ==
                                                                     id).AsEnumerable().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = id });
            }
            
            string collectionName = "Customers";
            var collection = client.CreateDocumentCollectionQuery(database.CollectionsLink).
                    Where(c => c.Id == collectionName).AsEnumerable().FirstOrDefault();
            if (collection == null)
            {
                collection = await
                    client.CreateDocumentCollectionAsync(database.CollectionsLink,
                        new DocumentCollection { Id = collectionName});
            }
            
            var contoso = new Customer
            {
                CustomerName = "Contoso Corp",
                PhoneNumbers = new PhoneNumber[]
                {
                    new PhoneNumber
                    {
                        CountryCode = "1",
                        AreaCode = "619",
                        MainNumber = "555-1212" },
                    new PhoneNumber
                    {
                        CountryCode = "1",
                        AreaCode = "760",
                        MainNumber = "555-2442" },
                }
            };
            var wwi = new Customer
            {
                CustomerName = "World Wide Importers",
                PhoneNumbers = new PhoneNumber[]
                {
                    new PhoneNumber
                    {
                        CountryCode = "1",
                        AreaCode = "858",
                        MainNumber = "555-7756" },
                    new PhoneNumber
                    {
                        CountryCode = "1",
                        AreaCode = "858",
                        MainNumber = "555-9142" },
                }
            };
            await client.CreateDocumentAsync(collection.DocumentsLink, contoso);
            await client.CreateDocumentAsync(collection.DocumentsLink, wwi);
            
            var customers = client.CreateDocumentQuery<Customer>
                    (collection.DocumentsLink).
                Where(c => c.CustomerName == "Contoso Corp").ToList();
            System.Console.WriteLine(customers.Count);
        }
    }
}