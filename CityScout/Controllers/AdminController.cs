using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;

namespace CityScout.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="1")]
    public class AdminController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IAuthService _authService;


        public AdminController(IProfileService profileService, IAuthService authService)
        {
            _profileService = profileService;
            _authService = authService;
        }

        [HttpPut("activate/{userId}")]
        public async Task<IActionResult> ActivateAccount(string userId)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(userId);
                if (profile == null)
                    return NotFound("User not found");
                if (profile.IsActive.HasValue && profile.IsActive.Value)
                    return BadRequest("Account is already activated");
                var result = await _profileService.SetAccountActiveStatusAsync(userId, true);
                if (!result)
                    return BadRequest("Activation failed");
                return Ok("Account activated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }

        [HttpPut("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateAccount(string userId)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(userId);
                if (profile == null)
                    return NotFound("User not found");
                if (profile.IsActive.HasValue && !profile.IsActive.Value)
                    return BadRequest("Account is already deactivated");
                var result = await _profileService.SetAccountActiveStatusAsync(userId, false);
                if (!result)
                    return BadRequest("Deactivation failed");
                return Ok("Account deactivated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccountList()
        {
            try
            {
                var accounts = await _profileService.GetAccountListAsync();
                if (accounts == null || accounts.Count == 0)
                    return NotFound("No accounts found");

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("promote-to-admin")]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            try
            {
                var result = await _authService.PromoteToAdmin(userId);
                if (result > 0)
                {
                    return Ok("User updated to admin");
                }
                return BadRequest(new { message = "Update failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
