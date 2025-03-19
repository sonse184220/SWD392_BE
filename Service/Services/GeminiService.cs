using LiteDB;
using Microsoft.Extensions.Configuration;
using Repository.ViewModels;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
namespace Service.Services
{
    public class GeminiService: IGeminiService
    {
        
            private readonly HttpClient _httpClient;
            private readonly string _apiKey;

            public GeminiService(HttpClient httpClient, IConfiguration configuration)
            {
                _httpClient = httpClient;
                _apiKey = configuration["Gemini:ApiKey"];
            }

            private async Task<string> SendGeminiRequest(string message)
            {
                var request = new
                {
                    contents = new[]
                    {
                    new { parts = new[] { new { text = message } } }
                }
                };

                var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

                try
                {
                    var response = await _httpClient.PostAsync(apiUrl, content);
                    var result = await response.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(result);
                    return doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error calling api: {ex.Message}");
                    return string.Empty;
                }
            }

            public async Task<List<ApiRecommendationVM.DestinationDTO>> GetDestinationRecommendation(string message)
            {
                string jsonResponse = await SendGeminiRequest($"{message}\n{GeneratePrompt()}");

                if (string.IsNullOrWhiteSpace(jsonResponse))
                    return new List<ApiRecommendationVM.DestinationDTO>();

                jsonResponse = CleanJsonResponse(jsonResponse);

                try
                {
                    var finalResult = System.Text.Json.JsonSerializer.Deserialize<List<ApiRecommendationVM.DestinationDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return finalResult ?? new List<ApiRecommendationVM.DestinationDTO>();
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"error parse: {jsonEx.Message}");
                    Console.WriteLine($" response json: {jsonResponse}");
                    return new List<ApiRecommendationVM.DestinationDTO>();
                }
            }

            public async Task<string> SendRequest(string message)
            {
                return await SendGeminiRequest(message);
            }

            private string CleanJsonResponse(string jsonResponse)
            {
                jsonResponse = jsonResponse.Trim();
                if (jsonResponse.StartsWith("\"") && jsonResponse.EndsWith("\""))
                {
                    jsonResponse = jsonResponse.Trim('"');
                    jsonResponse = jsonResponse.Replace("\\\"", "\"");
                }

                jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();
                return jsonResponse;
            }

            private string GeneratePrompt()
            {
                return @$"
Hãy trả về theo format sau, không thêm bất cứ chữ nào khác:
[
    {{
        ""destinationId"": 15047cdf-14fc-4dd2-a59f-6569ae4bc5f7,
        ""destinationName"": ""Da Nang Beach"",
        ""address"": ""Vo Nguyen Giap"",
        ""description"": ""A beautiful beach in Da Nang"",
        ""rate"": 4.8,
        ""categoryId"":5b841029-2be8-4f80-ae76-95d5c3feb8d6 ,
        ""ward"": ""Phuoc My"",
        ""status"": ""Open"",
        ""districtId"": 15047cdf-14fc-4dd2-a59f-6569ae4bc5f7,
        ""district"": {{
            ""districtId"": 15047cdf-14fc-4dd2-a59f-6569ae4bc5f7,
            ""name"": ""Son Tra"",
            ""description"": ""A district in Da Nang"",
            ""cityId"": 15047cdf-14fc-4dd2-a59f-6569ae4bc5f7,
            ""city"": {{
                ""cityId"": 15047cdf-14fc-4dd2-a59f-6569ae4bc5f7,
                ""name"": ""Da Nang"",
                ""description"": ""A beautiful coastal city""
            }}
        }}
    }}
]";
            }
        }

    
}
