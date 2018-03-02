using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;

namespace Azure.GraphDB
{
    class Program
    {
        static void Main(string[] args)
        {
            using (DocumentClient client = new DocumentClient(
                new Uri("https://ngtgraph1.documents.azure.com:443/"),
                "",
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                }))
            {
                ManageGraphDatabase(client).Wait();
            }
            
        }

        private static async Task ManageGraphDatabase(DocumentClient client)
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database {Id = "graphdb"});

            DocumentCollection graph = await
                client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri("graphdb"),
                    new DocumentCollection {Id = "graph"},
                    new RequestOptions {OfferThroughput = 1000});
            IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, "g.V().count()");
            while (query.HasMoreResults)
            {
                foreach (dynamic result in await query.ExecuteNextAsync())
                {
                    Console.WriteLine($"\t{JsonConvert.SerializeObject(result)}");
                }
            }
        }
    }
}