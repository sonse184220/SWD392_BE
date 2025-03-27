using Firebase.Database;
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
        private  IChatClient _aiClient;
        private readonly string firebaseurl;
        public AIService(IConfiguration config)
        {
            _config = config;
            //string endpoint = _config.GetValue<string>("Ollama:Endpoint");
            //string model = _config.GetValue<string>("Ollama:Model");
            firebaseurl = config["Firebase:FBDataBase"];
            //_aiClient = new OllamaChatClient(endpoint, model);
        }


        private async Task<string> GetFireBaseKeyAsync(string key)
        {
            string firebaseDatabaseUrl = firebaseurl.TrimEnd('/');
            var firebaseClient = new FirebaseClient(firebaseDatabaseUrl);

            var getConfig = await firebaseClient
                .Child(key)
                .OnceSingleAsync<string>();

            if (getConfig == null)
                throw new Exception("config not found in real time db");

            return getConfig.Trim();
        }
        public async Task<(string Summary, string Query)> GetAIResponse(string userPrompt, DatabaseSchema dbSchema)
        {
            string endpoint = await GetFireBaseKeyAsync("ollamaEndPoint");
            string model = await GetFireBaseKeyAsync("aiModel");
            _aiClient = new OllamaChatClient(endpoint, model);

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
            builder.AppendLine("You must return destination with this exact order of fields: DestinationID, destinationName, address, description, rate, categoryID, ward, status, categoryName, districtName, imageUrl");
            builder.AppendLine("- Use Vietnamese names for categories, wards, cities, and districts. Prefix conditions with N' for Unicode strings (e.g., N'Đà Lạt'). Do NOT add extra N' prefixes if the value already has it (e.g., do NOT write N'N'Đà Lạt').");
            builder.AppendLine("- The city name 'Da Lat' in prompts refers to 'Đà Lạt' in the City table. 'HCM' or 'Ho Chi Minh' or 'tp hcm' or anything closely resemble refers to 'TP Hồ Chí Minh'. 'Nha trang', 'nha trang', 'Nhatrang' refers to 'Nha Trang'");
            builder.AppendLine("- If the prompt contains a city, district, ward, or category that is NOT in the available lists above, generate a query using the exact name provided in the prompt (with N' prefix), but set the summary to indicate that no results will be found.");
            builder.AppendLine("- The SELECT statement MUST include ALL of the following columns in this exact order: DestinationID, DestinationName, Address, Description, Rate, CategoryID, Ward, Status, CategoryName, DistrictName, ImageUrl."); // Added ImageUrl, removed OpenTime, CloseTime
            builder.AppendLine("- If the prompt specifies a category (e.g., 'tourist attractions', 'cafes', 'restaurants'), map the category to the corresponding CategoryName from the Available Category Names list and filter by that CategoryName in the query (e.g., c.Name = N'Tôn giáo và văn hóa' for 'religion and culture', c.Name = N'Thư giãn' for 'entertainment' or 'relax'). If the category is unclear or not in the list, include a note in the summary indicating the category was not recognized and do not apply a category filter.");
            builder.AppendLine("- Your response MUST be a single line of valid JSON in the format: {\"summary\": \"your-summary\", \"query\": \"your-query\"}. Do NOT return any other format, such as a list of destinations or a different JSON structure. Non-compliant responses will cause an error.");
            builder.AppendLine("Examples:");
            builder.AppendLine("Prompt: 'cho tôi những địa điểm du lịch ở hcm' -> {\"summary\": \"Yes, there are interesting tourist attractions in TP Hồ Chí Minh: Nhà thờ Đức Bà, Bến Bạch Đằng.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, d.ImageUrl FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'TP Hồ Chí Minh' AND c.Name = N'Địa điểm du lịch' AND d.Status = 'Active'\"}"); // Removed OpenTime, CloseTime, and OpeningHours JOIN; Added d.ImageUrl
            builder.AppendLine("Prompt: 'find cafes in Da Lat' -> {\"summary\": \"Cafes in Đà Lạt: Cafe Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, d.ImageUrl FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Đà Lạt' AND c.Name = N'Cafe' AND d.Status = 'Active'\"}");
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




//builder.AppendLine("Notes:");
//builder.AppendLine("- Use Vietnamese names for categories, wards, cities, and districts. Prefix conditions with N' for Unicode strings (e.g., N'Đà Lạt'). Do NOT add extra N' prefixes if the value already has it (e.g., do NOT write N'N'Đà Lạt').");
//builder.AppendLine("- The city name 'Da Lat' in prompts refers to 'Đà Lạt' in the City table. 'HCM' or 'Ho Chi Minh' refers to 'TP Hồ Chí Minh'.");
//builder.AppendLine("- If the prompt contains a city, district, ward, or category that is NOT in the available lists above, generate a query using the exact name provided in the prompt (with N' prefix), but set the summary to indicate that no results will be found. For example, if the prompt is 'at city Bình Định' and Bình Định is not in Available City Names, return a summary like 'No specific results for Bình Định as it is not listed in the available cities.' and a query like 'SELECT ... WHERE ct.Name = N'Bình Định' ...' which will return no results.");
//builder.AppendLine("- The SELECT statement MUST include ALL of the following columns in this exact order: DestinationID, DestinationName, Address, Description, Rate, CategoryID, Ward, Status, CategoryName, DistrictName, OpenTime, CloseTime. Do NOT omit any columns, and do NOT change the order of these columns.");
//builder.AppendLine("- When joining with the OpeningHours table, a Destination might have multiple OpeningHours entries or none at all. To avoid duplicates and include destinations without OpeningHours, use a LEFT JOIN with a subquery: LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1. Do NOT add oh.IsClosed = 0 to the WHERE clause, as it is handled in the subquery.");
//builder.AppendLine("- If the prompt specifies a category (e.g., 'tourist attractions', 'cafes', 'restaurants'), map the category to the corresponding CategoryName from the Available Category Names list and filter by that CategoryName in the query (e.g., c.Name = N'Địa điểm du lịch' for 'tourist attractions', c.Name = N'Cafe' for 'cafes'). If the prompt uses a keyword that matches or closely resembles an Available Category Name (e.g., 'địa điểm du lịch', 'cafe', 'quán cà phê'), use the exact CategoryName from the list. If the category is unclear or not in the list, include a note in the summary indicating the category was not recognized and do not apply a category filter.");
//builder.AppendLine("Examples of valid prompts and responses:");
//builder.AppendLine("Prompt: 'cho tôi những địa điểm du lịch ở hcm' -> {\"summary\": \"Yes, there are interesting tourist attractions in TP Hồ Chí Minh: Nhà thờ Đức Bà, Bến Bạch Đằng.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'TP Hồ Chí Minh' AND c.Name = N'Địa điểm du lịch' AND d.Status = 'Active'\"}");
//builder.AppendLine("Prompt: 'I am at Da Lat. Is there any place to go?' -> {\"summary\": \"Yes, there are interesting places in Đà Lạt: Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Đà Lạt' AND d.Status = 'Active'\"}");
//builder.AppendLine("Prompt: 'at ward Lạc Dương' -> {\"summary\": \"Locations in Lạc Dương: Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE d.Ward = N'Lạc Dương' AND d.Status = 'Active'\"}");
//builder.AppendLine("Prompt: 'find cafes in Da Lat' -> {\"summary\": \"Cafes in Đà Lạt: Cafe Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Đà Lạt' AND c.Name = N'Cafe' AND d.Status = 'Active'\"}");
//builder.AppendLine("Prompt: 'at city Bình Định' -> {\"summary\": \"No specific results for Bình Định as it is not listed in the available cities.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Bình Định' AND d.Status = 'Active'\"}");
//builder.AppendLine("Prompt: 'find restaurants in HCM' -> {\"summary\": \"Restaurants in TP Hồ Chí Minh: Restaurant A.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'TP Hồ Chí Minh' AND c.Name = N'Restaurant' AND d.Status = 'Active'\"}");
//builder.AppendLine("Prompt: 'find unknown category in Da Lat' -> {\"summary\": \"The category 'unknown category' is not recognized. Showing all active locations in Đà Lạt.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Đà Lạt' AND d.Status = 'Active'\"}");
//builder.AppendLine("Your response MUST be a single line of valid JSON in the format: {\"summary\": \"your-summary\", \"query\": \"your-query\"}.");
//builder.AppendLine("The 'summary' should be a concise, human-readable answer listing interesting locations, or a message if none are found or if the location/category is unknown.");
//builder.AppendLine("The 'query' should use SQL Server syntax, join necessary tables, filter by location or category if specified, ensure the destination is active (Status = 'Active'), and check for open hours (IsClosed = 0) if relevant, limiting to TOP 100 rows.");
//builder.AppendLine("Include columns in this exact order: DestinationID, DestinationName, Address, Description, Rate, CategoryID, Ward, Status, CategoryName, DistrictName, OpenTime, CloseTime.");
//builder.AppendLine("If the prompt is unclear or no data matches, return: {\"summary\": \"Unable to find locations for the given prompt.\", \"query\": \"\"}");


//builder.AppendLine("You must return destination with this exact order of fields: DestinationID, destinationName, address, description, rate, categoryID, ward, status, categoryName, districtName, openTime, closeTime, imageUrl");
//builder.AppendLine("- Use Vietnamese names for categories, wards, cities, and districts. Prefix conditions with N' for Unicode strings (e.g., N'Đà Lạt'). Do NOT add extra N' prefixes if the value already has it (e.g., do NOT write N'N'Đà Lạt').");
//builder.AppendLine("- The city name 'Da Lat' in prompts refers to 'Đà Lạt' in the City table. 'HCM' or 'Ho Chi Minh' or 'tp hcm' or anything closely resemble refers to 'TP Hồ Chí Minh'. 'Nha trang', 'nha trang', 'Nhatrang' refers to 'Nha Trang'");
//builder.AppendLine("- If the prompt contains a city, district, ward, or category that is NOT in the available lists above, generate a query using the exact name provided in the prompt (with N' prefix), but set the summary to indicate that no results will be found.");
//builder.AppendLine("- The SELECT statement MUST include ALL of the following columns in this exact order: DestinationID, DestinationName, Address, Description, Rate, CategoryID, Ward, Status, CategoryName, DistrictName, OpenTime, CloseTime.");
//builder.AppendLine("- When joining with the OpeningHours table, use a LEFT JOIN to avoid excluding destinations without OpeningHours: LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1.");
//builder.AppendLine("- If the prompt specifies a category (e.g., 'tourist attractions', 'cafes', 'restaurants'), map the category to the corresponding CategoryName from the Available Category Names list and filter by that CategoryName in the query (e.g., c.Name = N'Địa điểm du lịch' for 'tourist attractions', c.Name = N'Cafe' for 'cafes'). If the category is unclear or not in the list, include a note in the summary indicating the category was not recognized and do not apply a category filter.");
//builder.AppendLine("- Your response MUST be a single line of valid JSON in the format: {\"summary\": \"your-summary\", \"query\": \"your-query\"}. Do NOT return any other format, such as a list of destinations or a different JSON structure. Non-compliant responses will cause an error.");
//builder.AppendLine("Examples:");
//builder.AppendLine("Prompt: 'cho tôi những địa điểm du lịch ở hcm' -> {\"summary\": \"Yes, there are interesting tourist attractions in TP Hồ Chí Minh: Nhà thờ Đức Bà, Bến Bạch Đằng.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'TP Hồ Chí Minh' AND c.Name = N'Địa điểm du lịch' AND d.Status = 'Active'\"}");
//builder.AppendLine("Prompt: 'find cafes in Da Lat' -> {\"summary\": \"Cafes in Đà Lạt: Cafe Langbiang.\", \"query\": \"SELECT TOP 100 d.DestinationID, d.DestinationName, d.Address, d.Description, d.Rate, d.CategoryID, d.Ward, d.Status, c.Name AS CategoryName, dt.Name AS DistrictName, oh.OpenTime, oh.CloseTime FROM Destination d JOIN District dt ON d.DistrictID = dt.DistrictID JOIN City ct ON dt.CityID = ct.CityID LEFT JOIN (SELECT *, ROW_NUMBER() OVER (PARTITION BY DestinationID ORDER BY OpenTime) AS rn FROM OpeningHours WHERE IsClosed = 0) oh ON d.DestinationID = oh.DestinationID AND oh.rn = 1 JOIN Category c ON d.CategoryID = c.CategoryID WHERE ct.Name = N'Đà Lạt' AND c.Name = N'Cafe' AND d.Status = 'Active'\"}");