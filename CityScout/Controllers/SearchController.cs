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
                    return Ok(new { Summary = "No interesting locations found.", Results = new List<object>() });
                }

                // Generate summary line from the DestinationName column (index 1)
                var names = data.Skip(1).Select(row => row[1]); // Skip headers, take DestinationName
                var finalSummary = names.Any()
                    ? $"Yes, there are {names.Count()} interesting locations: {string.Join(", ", names)}."
                    : "No interesting locations found.";

                // Structure results for the frontend
                var results = data.Skip(1).Select(row => new
                {
                    DestinationID = row[0],
                    DestinationName = row[1],
                    Address = row[2],
                    Description = row[3],
                    Rate = row[4],
                    CategoryID = row[5],
                    CategoryName = row[6],
                    DistrictName = row[7],
                    OpenTime = row.ElementAtOrDefault(8), // Optional
                    CloseTime = row.ElementAtOrDefault(9)  // Optional
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
    }

    public class SearchQueryRequest
    {
        public string SearchPrompt { get; set; }
    }
}