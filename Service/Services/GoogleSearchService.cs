using Microsoft.Extensions.Configuration;
using Repository.ViewModels;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Services
{
    public class GoogleSearchService: IGoogleSearchService
    {
        private readonly HttpClient _htppClient;
        private string _apiKey;
        private string _cxKey;
        public GoogleSearchService(HttpClient htppClient, IConfiguration _configuration)
        {
            _htppClient = htppClient;
            _apiKey = _configuration["GoogleSearch:ApiKey"];
            _cxKey = _configuration["GoogleSearch:CxKey"];

        }
        public async Task<List<string>> SearchImagesAsync(string query,int expectedResult)
        {
            string requestUrl = $"https://www.googleapis.com/customsearch/v1?q={query}&searchType=image&key={_apiKey}&cx={_cxKey}&num={expectedResult}";

            var response = await _htppClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode) return new List<string>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var searchResults = JsonSerializer.Deserialize<GoogleSearchResponse>(jsonResponse);

            var imageUrls = new List<string>();
            if (searchResults?.Items != null)
            {
                foreach (var item in searchResults.Items)
                {
                    imageUrls.Add(item.Link);
                }
            }

            return imageUrls;
        }
    }
}
