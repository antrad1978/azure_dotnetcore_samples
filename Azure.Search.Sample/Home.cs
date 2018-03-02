using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace Azure.Search.Sample
{
    // The SerializePropertyNamesAsCamelCase attribute is defined in the Azure
    // Search .NET SDK.
    // It ensures that Pascal-case property names in the model class are mapped to
    // camel-case field names in the index.
    [SerializePropertyNamesAsCamelCase]
    public partial class Home
    {
        [IsFilterable]
        [System.ComponentModel.DataAnnotations.Key]
        public string HomeId { get; set; }
        [IsFilterable, IsSortable, IsFacetable]
        public double? RetailPrice { get; set; }
        [IsFilterable, IsSortable, IsFacetable]
        public int? SquareFootage { get; set; }
        [IsSearchable]
        public string Description { get; set; }
        [IsFilterable, IsSortable]
        public GeographyPoint Location { get; set; }
    }
}