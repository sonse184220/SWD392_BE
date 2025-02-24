using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.RequestModels;
using Repository.ViewModels;
using Service.Interfaces;

namespace CityScout.Controllers
{
    [Route("cityscout/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IFcmService _fcmService;
        public NotificationController(IFcmService fcmService)
        {
            _fcmService = fcmService;
        }
        
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] PushNotificationRequest request)
        {
            if (string.IsNullOrEmpty(request.DeviceToken))
            {
                return BadRequest(new { message = "Device token is required." });
            }
            NotificationViewModel result = await _fcmService.SendPushNotificationAsync(request.DeviceToken,request.Title,request.Body);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
            
        }
    }
}
