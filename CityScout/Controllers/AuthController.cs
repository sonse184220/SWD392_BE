using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.RequestModels;
using Service.Interfaces;

namespace CityScout.Controllers
{
    [Route("cityscout/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("firebase")]
        public async Task<IActionResult> Authenticate([FromBody] FirebaseTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.FirebaseToken))
            {
                return BadRequest("Token is required");
            }
            try
            {
                var response = await _authService.AuthenticateWithFirebaseAsync(request);
                return Ok(response);
            }catch(Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }

        }
      

    }

}
