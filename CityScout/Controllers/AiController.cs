using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.RequestModels;
using Service.Interfaces;

namespace CityScout.Controllers
{
    [Route("cityscout/ai")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        public AiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
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
            var apiResponse = await _geminiService.SendRequest(request.message);
            return Ok(apiResponse);
        }
    }
}
