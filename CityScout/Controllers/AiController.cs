using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Repository.RequestModels;
using Service.Interfaces;
using Service.Services;

namespace CityScout.Controllers
{
    [Route("cityscout/ai")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly IRedisCacheService _cache;
        public AiController(IGeminiService geminiService, IRedisCacheService cache)
        {
            _geminiService = geminiService;
            _cache = cache;
        }
        [HttpPost("get-recommendation")]
        public async Task<IActionResult> GetRecommendationFromAi([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.message)){
                return BadRequest("Message can not be null or empty");
            }



            var apiResponse = await _geminiService.GetDestinationRecommendation(request.message);
            return Ok(apiResponse);
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> ChatWithAI([FromBody] ChatRequest request)
        {
            
            if (string.IsNullOrEmpty(request.message))
            {
                return BadRequest("Message can not be null or empty");
            }
            string cacheKey = $"gemini:{request.message}";
            var cachedResponse = await _cache.GetCacheAsync<string>(cacheKey);
            if (cachedResponse != null)
            {
                return Ok(new { fromCache = true, response = cachedResponse });
            }
            var apiResponse = await _geminiService.SendRequest(request.message);
            await _cache.SetCacheAsync(cacheKey, apiResponse, TimeSpan.FromMinutes(20));

            return Ok(new { fromCache = false, response = apiResponse });
            //return Ok(apiResponse);
        }
    }
}
