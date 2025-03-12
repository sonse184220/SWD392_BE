using Microsoft.AspNetCore.Mvc;
using CityScout.Services;
using CityScout.DTOs;

namespace CityScout.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService _destinationService;

        public DestinationController(IDestinationService destinationService)
        {
            _destinationService = destinationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDestinations()
        {
            var result = await _destinationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDestination(string id)
        {
            var destination = await _destinationService.GetByIdAsync(id);
            if (destination == null) return NotFound();
            return Ok(destination);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDestination([FromBody] DestinationCreateDto dto)
        {
            var createdId = await _destinationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDestination), new { id = createdId }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDestination(string id, [FromBody] DestinationCreateDto dto)
        {
            await _destinationService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(string id)
        {
            var success = await _destinationService.RemoveAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
