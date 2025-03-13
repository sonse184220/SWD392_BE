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
        public async Task<IActionResult> UpdateProfile(
      string id,
      [FromForm] UpdateProfileRequest request,
      IFormFile? profilePicture)
        {
            try
            {
                string? profilePictureData = null;

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await profilePicture.CopyToAsync(ms);
                    profilePictureData = $"data:{profilePicture.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
                }

                var result = await _profileService.UpdateProfileAsync(id, request, profilePictureData);
                if (!result)
                    return NotFound("User not found");

                return Ok("Profile updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }






        [HttpGet("{id}")]
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
                return StatusCode(500, ex.Message);
            }
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
    }
}
