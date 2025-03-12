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
            var result = await _cityService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(string id)
        {
            var city = await _cityService.GetByIdAsync(id);
            if (city == null) return NotFound();
            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityCreateDto dto)
        {
            var createdId = await _cityService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCity), new { id = createdId }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(string id, [FromBody] CityCreateDto dto)
        {
            await _cityService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(string id)
        {
            var success = await _cityService.RemoveAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
