using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;

namespace Azure.Search.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            String searchServiceName = "ngtsearch1";
            string accesskey = "E560A03E5044BB6F9A63338E4511F8E6";
            SearchServiceClient serviceClient = new
                SearchServiceClient(searchServiceName,
                    new SearchCredentials(accesskey));
            var definition = new Index()
            {
                Name = "homes",
                Fields = FieldBuilder.BuildForType<Home>()
            };
            serviceClient.Indexes.DeleteAsync("homes").Wait();
            serviceClient.Indexes.CreateOrUpdate(definition);

            var homes = new Home[]
            {
                new Home()
                {
                    HomeId = "133",
                    RetailPrice = Convert.ToDouble("459999.00"),
                    SquareFootage = 3200,
                    Description =
                        "Single floor, ranch style on 1 acre of property. 4 bedroom,large living room with open kitchen, dining area.",
                    Location = GeographyPoint.Create(47.678581, -122.131577)
                }
            };
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("homes");
            var batch = IndexBatch.Upload(homes);
            indexClient.Documents.Index(batch);
            
            SearchParameters parameters =
                    new SearchParameters()
                    {
                        Select = new[] { "SquareFootage" }
                    };
            DocumentSearchResult<Home> searchResults = indexClient.Documents.Search<Home>("3200", parameters);
            
            foreach (SearchResult<Home> result in searchResults.Results)
            {
                Console.WriteLine(result.Document);
            }
        }
    }
}