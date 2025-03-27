using Microsoft.AspNetCore.Mvc;
using CityScout.Services;
using Repository.ViewModels;

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
                var result = await _districtService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrict(string id)
        {
            try
            {
                var district = await _districtService.GetByIdAsync(id);
                if (district == null) return NotFound();
                return Ok(district);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] DistrictCreateDto dto)
        {
            try
            {
                var createdId = await _districtService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetDistrict), new { id = createdId }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistrict(string id, [FromBody] DistrictCreateDto dto)
        {
            try
            {
                await _districtService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistrict(string id)
        {
            try
            {
                var success = await _districtService.RemoveAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
