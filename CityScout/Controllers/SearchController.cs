using Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Service.ChatModel;

namespace CityScout.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly AIService _aiService;
        private readonly IDatabaseService _dbService;
        private DatabaseSchema _dbSchema;
        private readonly ILogger<SearchController> _logger;

        public SearchController(AIService aiService, IDatabaseService dbService, ILogger<SearchController> logger)
        {
            _aiService = aiService;
            _dbService = dbService;
            _logger = logger;
        }

        [HttpPost("query")]
        [AllowAnonymous]
        public async Task<IActionResult> Query([FromBody] SearchQueryRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.SearchPrompt))
            {
                return BadRequest(new { Error = "Search prompt is required." });
            }

            try
            {
                _dbSchema ??= await _dbService.GenerateSchema();
                var (summary, query) = await _aiService.GetAIResponse(request.SearchPrompt, _dbSchema);
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest(new { Error = "AI failed to generate a valid SQL query.", Summary = summary ?? "No summary provided." });
                }



                var data = await _dbService.GetDataTable(query);
                if (data == null || data.Count <= 1) // No data beyond headers
                {
                    // Use the summary from AI response if no data is found
                    return Ok(new { Summary = summary, Results = new List<object>() });
                }


                // Verify column headers (first row of data)
                var expectedHeaders = new List<string>
        {
            "DestinationID", "DestinationName", "Address", "Description", "Rate",
            "CategoryID", "Ward", "Status", "CategoryName", "DistrictName"
            , "ImageUrl" // Added ImageUrl
        };
                var actualHeaders = data[0]; // First row contains headers
                if (!expectedHeaders.SequenceEqual(actualHeaders))
                {
                    _logger.LogWarning("Column headers do not match expected order. Expected: {Expected}, Actual: {Actual}",
                        string.Join(", ", expectedHeaders), string.Join(", ", actualHeaders));
                }

                // Generate summary line from the DestinationName column (index 1)
                var names = data.Skip(1).Select(row => row[1]); // Skip headers, take DestinationName
                var finalSummary = names.Any()
                    ? $"Yes, there are {names.Count()} interesting locations: {string.Join(", ", names)}."
                    : summary;

                // Structure results for the frontend
                var results = data.Skip(1).Select(row => new
                {
                    destinationId = row[0], // Changed to lowercase
                    DestinationName = row[1],
                    Address = row[2],
                    Description = row[3],
                    Rate = row[4],
                    CategoryID = row[5],
                    Ward = row[6],
                    Status = row[7],
                    CategoryName = row[8],
                    DistrictName = row[9],
                    ImageUrl = row.ElementAtOrDefault(10) // Added ImageUrl
                }).ToList();

                return Ok(new
                {
                    Summary = finalSummary,
                    Results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing search query: {Prompt}", request.SearchPrompt);
                return BadRequest(new { Error = $"Failed to process the search query. Error: {ex.Message}" });
            }
        }

        public class SearchQueryRequest
        {
            public string SearchPrompt { get; set; }
        }
    }
}