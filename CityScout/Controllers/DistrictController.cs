using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using CityScout.Services;

namespace CityScout.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDistricts()
        {
            try
            {
                var districts = await _districtService.GetAllAsync();
                return Ok(districts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrict(int id)
        {
            try
            {
                var district = await _districtService.GetByIdAsync(id);
                if (district == null)
                    return NotFound();

                return Ok(district);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] District district)
        {
            try
            {
                var createdId = await _districtService.CreateAsync(district);
                district.DistrictId = createdId;
                return CreatedAtAction(nameof(GetDistrict), new { id = district.DistrictId }, district);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistrict(int id, [FromBody] District district)
        {
            try
            {
                if (id != district.DistrictId)
                    return BadRequest("District ID mismatch.");

                await _districtService.UpdateAsync(district);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            try
            {
                var result = await _districtService.RemoveAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
