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
            var cities = await _cityService.GetAllAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id)
        {
            var city = await _cityService.GetByIdAsync(id);
            if (city == null)
                return NotFound();

            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityCreateDto cityDto)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] City city)
        {
            if (id != city.CityId)
                return BadRequest("City ID mismatch.");

            await _cityService.UpdateAsync(city);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var result = await _cityService.RemoveAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}