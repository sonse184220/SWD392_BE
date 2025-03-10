using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.RequestModels;
using Service.Interfaces;

namespace CityScout.Controllers
{
    [Route("cityscout/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                request.UserId = id;
                var result = await _profileService.UpdateProfileAsync(request);
                if (!result)
                    return BadRequest("Update failed");
                return Ok("Profile updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<IActionResult> GetProfile(string id)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(id);
                if (profile == null)
                    return NotFound("User not found");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
