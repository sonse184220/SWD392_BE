using Microsoft.AspNetCore.Mvc;
using CityScout.Services;
using CityScout.DTOs;
using Service.Interfaces;

namespace CityScout.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            try
            {
                var result = await _cityService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(string id)
        {
            try
            {
                var city = await _cityService.GetByIdAsync(id);
                if (city == null) return NotFound();
                return Ok(city);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityCreateDto dto)
        {
            try
            {
                var createdId = await _cityService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetCity), new { id = createdId }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(string id, [FromBody] CityCreateDto dto)
        {
            try
            {
                await _cityService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(string id)
        {
            try
            {
                var success = await _cityService.RemoveAsync(id);
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
