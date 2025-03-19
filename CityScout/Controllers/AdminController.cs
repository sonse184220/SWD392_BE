using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace CityScout.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="1")]
    public class AdminController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public AdminController(IProfileService profileService)
        {
            _profileService = profileService;
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
    }
}
