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
            request.UserId = id;
            var result = await _profileService.UpdateProfileAsync(request);
            if (!result)
                return BadRequest("Update failed");
            return Ok("Profile updated successfully");
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<IActionResult> GetProfile(string id)
        {
            var profile = await _profileService.GetProfileByIdAsync(id);
            if (profile == null)
                return NotFound("User not found");
            return Ok(profile);
        }
    }
}
