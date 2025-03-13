using Microsoft.AspNetCore.Mvc;
using CityScout.Services;
using CityScout.DTOs;

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
            var result = await _districtService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrict(string id)
        {
            var district = await _districtService.GetByIdAsync(id);
            if (district == null) return NotFound();
            return Ok(district);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] DistrictCreateDto dto)
        {
            var createdId = await _districtService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDistrict), new { id = createdId }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistrict(string id, [FromBody] DistrictCreateDto dto)
        {
            await _districtService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistrict(string id)
        {
            var success = await _districtService.RemoveAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
