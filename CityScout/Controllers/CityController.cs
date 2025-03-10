using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Repository.Models;
using CityScout.DTOs;

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
                var cities = await _cityService.GetAllAsync();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id)
        {
            try
            {
                var city = await _cityService.GetByIdAsync(id);
                if (city == null)
                    return NotFound();

                return Ok(city);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityCreateDto cityDto)
        {
            try
            {
                var city = new City
                {
                    CityId = cityDto.CityId,
                    Name = cityDto.Name,
                    Description = cityDto.Description
                };

                var createdId = await _cityService.CreateAsync(city);

                return CreatedAtAction(nameof(GetCity), new { id = createdId }, city);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] City city)
        {
            try
            {
                if (id != city.CityId)
                    return BadRequest("City ID mismatch.");

                await _cityService.UpdateAsync(city);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            try
            {
                var result = await _cityService.RemoveAsync(id);
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