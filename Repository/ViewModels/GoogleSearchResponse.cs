using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class GoogleSearchResponse
    {
        [JsonPropertyName("items")]
        public List<SearchItem> Items { get; set; }
    }
    public class SearchItem
    {
        [JsonPropertyName("link")]

        public string Link { get; set; }
    }
}
