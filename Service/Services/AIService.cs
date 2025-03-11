using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Service.ChatModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Service.Services
{
    public class AIService
    {
        private readonly IConfiguration _config;
        private readonly IChatClient _aiClient;

        public AIService(IConfiguration config)
        {
            _config = config;
            string endpoint = _config.GetValue<string>("Ollama:Endpoint");
            string model = _config.GetValue<string>("Ollama:Model");
            _aiClient = new OllamaChatClient(endpoint, model);
        }

        public async Task<(string Summary, string Query)> GetAIResponse(string userPrompt, DatabaseSchema dbSchema)
        {
            var builder = new StringBuilder();

            builder.AppendLine("You are a SQL query generation assistant for a SQL Server database. Your task is to generate a SQL query based on the provided database schema and user prompt.");
            builder.AppendLine("Use the following database schema to create SQL queries:");
            foreach (var table in dbSchema.SchemaRaw)
            {
                builder.AppendLine(table);
            }
            builder.AppendLine("Notes:");
            builder.AppendLine("- The city name 'Da Lat' in prompts refers to 'Đà Lạt' in the City table.");
            builder.AppendLine("Examples of valid prompts and responses:");
            builder.AppendLine("Prompt: 'I am at Da Lat. Is there any place to go?' -> {\"summary\": \"Yes, there are interesting places in Đà Lạt: Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID JOIN OpeningHours oh ON d.DestinationID = oh.DestinationID JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = 'Đà Lạt' AND d.Status = 'Active' AND oh.IsClosed = 0\"}");
            builder.AppendLine("Prompt: 'at location Lạc Dương' -> {\"summary\": \"Locations in Lạc Dương: Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN OpeningHours oh ON d.DestinationID = oh.DestinationID JOIN Category c ON d.CategoryID = c.CategoryID WHERE dt.Name = 'Lạc Dương' AND d.Status = 'Active' AND oh.IsClosed = 0\"}");
            builder.AppendLine("Prompt: 'list all destinations' -> {\"summary\": \"Listing all active destinations: Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, c.Name AS CategoryName, dt.Name AS DistrictName FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN Category c ON d.CategoryID = c.CategoryID WHERE d.Status = 'Active'\"}");
            builder.AppendLine("Your response MUST be a single line of valid JSON in the format: {\"summary\": \"your-summary\", \"query\": \"your-query\"}.");
            builder.AppendLine("Do NOT include any explanatory text, tags (e.g., <think>), or markdown (e.g., ```json) before or after the JSON. Output ONLY the JSON string.");
            builder.AppendLine("The 'summary' should be a concise, human-readable answer (one line) listing interesting locations, or a message if none are found.");
            builder.AppendLine("The 'query' should use SQL Server syntax, join necessary tables (e.g., City, District, Destination, OpeningHours, Category), filter by location if specified (e.g., CityName or DistrictName), ensure the destination is active (Status = 'Active'), and check for open hours (IsClosed = 0) if relevant, limiting to TOP 100 rows.");
            builder.AppendLine("Include columns: DestinationID, DestinationName, Address, Description, Rate, CategoryID, CategoryName, DistrictName, and OpenTime, CloseTime if OpeningHours is joined.");
            builder.AppendLine("If the prompt is unclear or no data matches, return: {\"summary\": \"Unable to find locations for the given prompt.\", \"query\": \"\"}");

            var chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, "You must output only valid JSON with no additional text, tags, or markdown. Non-JSON output will cause an error."),
                new ChatMessage(ChatRole.User, builder.ToString()),
                new ChatMessage(ChatRole.User, userPrompt)
            };

            var response = await _aiClient.CompleteAsync(chatHistory);
            var rawResponse = response.Message.Text;

            // Log the raw response for debugging
            Console.WriteLine($"Raw AI response: {rawResponse}");

            // Clean up the response
            var responseContent = rawResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Replace("\n", " ")
                .Replace("\\n", " ")
                .Replace("<think>", "")
                .Replace("</think>", "")
                .Trim();

            // Extract JSON using regex
            var jsonMatch = Regex.Match(responseContent, @"^\s*\{\s*""summary""\s*:\s*"".*""\s*,\s*""query""\s*:\s*"".*""\s*\}\s*$");
            if (jsonMatch.Success)
            {
                responseContent = jsonMatch.Value;
            }
            else
            {
                jsonMatch = Regex.Match(responseContent, @"\{(?:[^{}]|(?:\{[^{}]*\}))*\}");
                if (jsonMatch.Success)
                {
                    responseContent = jsonMatch.Value;
                }
                else
                {
                    throw new InvalidOperationException($"No valid JSON found in AI response. Raw response: {rawResponse}");
                }
            }

            try
            {
                // Deserialize into a dictionary
                var jsonDict = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                if (jsonDict == null || !jsonDict.ContainsKey("summary") || !jsonDict.ContainsKey("query"))
                {
                    throw new InvalidOperationException($"AI response does not contain required fields (summary, query). Cleaned response: {responseContent}");
                }

                return (jsonDict["summary"], jsonDict["query"]);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse AI response as JSON. Cleaned response: {responseContent}. Error: {ex.Message}");
            }
        }
    }
}