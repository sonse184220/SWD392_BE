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
            //foreach (var table in dbSchema.SchemaRaw)
            //{
            //    builder.AppendLine(table);
            //}

            foreach (var table in dbSchema.SchemaRaw)
            {
                builder.AppendLine(table);
            }

            // Append city names
            builder.AppendLine("Available City Names:");
            builder.AppendLine(string.Join(", ", dbSchema.CityNames.Select(name => $"N'{name}'")));

            // Append district names
            builder.AppendLine("Available District Names:");
            builder.AppendLine(string.Join(", ", dbSchema.DistrictNames.Select(name => $"N'{name}'")));

            // Append ward names
            builder.AppendLine("Available Ward Names:");
            builder.AppendLine(string.Join(", ", dbSchema.WardNames.Select(name => $"N'{name}'")));

            // Append category names
            builder.AppendLine("Available Category Names:");
            builder.AppendLine(string.Join(", ", dbSchema.CategoryNames.Select(name => $"N'{name}'")));

            builder.AppendLine("Notes:");
            builder.AppendLine("- Use Vietnamese names for categories, wards, cities, and districts. Prefix conditions with N' for Unicode strings (e.g., N'Đà Lạt'). Do NOT add extra N' prefixes if the value already has it (e.g., do NOT write N'N'Đà Lạt').");
            builder.AppendLine("- The city name 'Da Lat' in prompts refers to 'Đà Lạt' in the City table. 'HCM' or 'Ho Chi Minh' refers to 'TP Hồ Chí Minh'.");
            builder.AppendLine("- If the prompt contains a city, district, ward, or category that is NOT in the available lists above, generate a query using the exact name provided in the prompt (with N' prefix), but set the summary to indicate that no results will be found. For example, if the prompt is 'at city Bình Định' and Bình Định is not in Available City Names, return a summary like 'No specific results for Bình Định as it is not listed in the available cities.' and a query like 'SELECT ... WHERE ct.Name = N'Bình Định' ...' which will return no results.");
            builder.AppendLine("- The SELECT statement MUST include columns in this exact order: DestinationID, DestinationName, Address, Description, Rate, CategoryID, Ward, Status, CategoryName, DistrictName, OpenTime, CloseTime. Do NOT change the order of these columns.");
            builder.AppendLine("Examples of valid prompts and responses:");
            builder.AppendLine("Prompt: 'I am at Da Lat. Is there any place to go?' -> {\"summary\": \"Yes, there are interesting places in Đà Lạt: Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID JOIN OpeningHours oh ON d.DestinationID = oh.DestinationID JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Đà Lạt' AND d.Status = 'Active' AND oh.IsClosed = 0\"}");
            builder.AppendLine("Prompt: 'at ward Lạc Dương' -> {\"summary\": \"Locations in Lạc Dương: Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN OpeningHours oh ON d.DestinationID = oh.DestinationID JOIN Category c ON d.CategoryID = c.CategoryID WHERE d.Ward = N'Lạc Dương' AND d.Status = 'Active' AND oh.IsClosed = 0\"}");
            builder.AppendLine("Prompt: 'find cafes in Da Lat' -> {\"summary\": \"Cafes in Đà Lạt: Cafe Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID JOIN OpeningHours oh ON d.DestinationID = oh.DestinationID JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Đà Lạt' AND c.Name = N'Cafe' AND d.Status = 'Active' AND oh.IsClosed = 0\"}");
            builder.AppendLine("Prompt: 'at city Bình Định' -> {\"summary\": \"No specific results for Bình Định as it is not listed in the available cities.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID JOIN OpeningHours oh ON d.DestinationID = oh.DestinationID JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Bình Định' AND d.Status = 'Active' AND oh.IsClosed = 0\"}");
            builder.AppendLine("Your response MUST be a single line of valid JSON in the format: {\"summary\": \"your-summary\", \"query\": \"your-query\"}.");
            builder.AppendLine("The 'summary' should be a concise, human-readable answer listing interesting locations, or a message if none are found or if the location/category is unknown.");
            builder.AppendLine("The 'query' should use SQL Server syntax, join necessary tables, filter by location or category if specified, ensure the destination is active (Status = 'Active'), and check for open hours (IsClosed = 0) if relevant, limiting to TOP 100 rows.");
            builder.AppendLine("Include columns: DestinationID, DestinationName, Address, Description, Ward, Status, Rate, CategoryID, CategoryName, DistrictName, and OpenTime, CloseTime if OpeningHours is joined.");
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
                var cleanedQuery = jsonDict["query"].Replace("N'N'", "N'");
                return (jsonDict["summary"], jsonDict["query"]);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse AI response as JSON. Cleaned response: {responseContent}. Error: {ex.Message}");
            }
        }
    }
}