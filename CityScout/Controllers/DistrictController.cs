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
            var districts = await _districtService.GetAllAsync();
            return Ok(districts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrict(int id)
        {
            var district = await _districtService.GetByIdAsync(id);
            if (district == null)
                return NotFound();

            return Ok(district);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] District district)
        {
            var createdId = await _districtService.CreateAsync(district);
            district.DistrictId = createdId; 
            return CreatedAtAction(nameof(GetDistrict), new { id = district.DistrictId }, district);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistrict(int id, [FromBody] District district)
        {
            if (id != district.DistrictId)
                return BadRequest("District ID mismatch.");

            await _districtService.UpdateAsync(district);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            var result = await _districtService.RemoveAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
