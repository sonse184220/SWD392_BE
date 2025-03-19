using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Repository.Models;
using Repository.RequestModels;
using Service.Interfaces;
using Service.Services;
using System.Security.Claims;
using System.Text.Json;

namespace CityScout.Controllers
{
    [Route("cityscout/ai")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly IRedisCacheService _cache;
        private readonly IGoogleSearchService _googleSearchService;
        public AiController(IGeminiService geminiService, IRedisCacheService cache, IGoogleSearchService googleSearchService)
        {
            _geminiService = geminiService;
            _cache = cache;
            _googleSearchService = googleSearchService;
        }
        //[Authorize]
        [HttpPost("get-recommendation")]
        public async Task<IActionResult> GetRecommendationFromAi([FromBody] ChatRequest request)
        {
            //if (string.IsNullOrEmpty(request.message)){
            //    return BadRequest("Message can not be null or empty");
            //}
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized();
            //}
            var userId = "0d8fcbdb-9183-4f9d-af22-9904314a288f";
            string cacheKey = $"recommendation:{userId}";


            var apiResponse = await _geminiService.GetDestinationRecommendation(request.message);


            foreach (var destination in apiResponse)
            {
                destination.imageUrls = await _googleSearchService.SearchImagesAsync(destination.DestinationName, 2);
            }




            var message = new
            {
                Prompt = request.message,
                Response = apiResponse,
                CreatedAt = DateTime.UtcNow.ToString("o")
            };

            string jsonMessage = JsonSerializer.Serialize(message);

            await _cache.ListLeftPushAsync(cacheKey, jsonMessage);

            await _cache.ListTrimAsync(cacheKey, 0, 20);

            await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(20));

            return Ok(message);
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> ChatWithAI([FromBody] ChatRequest request)
        {
            

            if (string.IsNullOrEmpty(request.message))
            {
                return BadRequest("Message can not be null or empty");
            }
            
            //var cachedResponse = await _cache.GetCacheAsync<string>(cacheKey);
            //if (cachedResponse != null)
            //{
            //    return Ok(new { fromCache = true, response = cachedResponse });
            //}
            //if (string.IsNullOrEmpty(request.message)){
            //    return BadRequest("Message can not be null or empty");
            //}
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized();
            //}
            var userId = "0d8fcbdb-9183-4f9d-af22-9904314a288f";
            string cacheKey = $"chat:{userId}";



            var apiResponse = await _geminiService.SendRequest(request.message);

            var message = new
            {
                Prompt = request.message,
                Response = apiResponse,
                CreatedAt = DateTime.UtcNow.ToString("o")
            };

            string jsonMessage = JsonSerializer.Serialize(message);

            await _cache.ListLeftPushAsync(cacheKey, jsonMessage);

            await _cache.ListTrimAsync(cacheKey, 0, 20);

            await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(20));
            //await _cache.SetCacheAsync(cacheKey, apiResponse, TimeSpan.FromMinutes(20));

            return Ok(message);
            //return Ok(apiResponse);
        }
        //[Authorize]
        [HttpGet("get-messages-recommendation")]
        public async Task<IActionResult> GetAllMessages()
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized();
            //}
            var userId = "0d8fcbdb-9183-4f9d-af22-9904314a288f";
            string cacheKey = $"recommendation:{userId}";
            var messages = await _cache.ListRangeAsync(cacheKey, 0, -1); 

            if (messages.Length == 0)
            {
                Ok("No message found");
            }
            var parsedMessage = messages.Select(m => JsonSerializer.Deserialize<Repository.ViewModels.GetRecommendationChatVM>(m)).ToList();
            return Ok(parsedMessage);
        }
        [HttpGet("get-messages-chat")]
        public async Task<IActionResult> GetAllChat()
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized();
            //}
            var userId = "0d8fcbdb-9183-4f9d-af22-9904314a288f";
            string cacheKey = $"chat:{userId}";
            var messages = await _cache.ListRangeAsync(cacheKey, 0, -1);

            if (messages.Length == 0)
            {
                Ok("No message found");
            }
            var parsedMessage = messages.Select(m => JsonSerializer.Deserialize<Repository.ViewModels.GetChatVM>(m)).ToList();
            return Ok(parsedMessage);
        }
    }
}
