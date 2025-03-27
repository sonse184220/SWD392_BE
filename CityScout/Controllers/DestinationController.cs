using Microsoft.AspNetCore.Mvc;
using CityScout.Services;
using Service.Interfaces;
using Repository.ViewModels;

namespace CityScout.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService _destinationService;
        private readonly ICloudinaryService _cloudinaryService;
        public DestinationController(IDestinationService destinationService, ICloudinaryService cloudinaryService)
        {
            _destinationService = destinationService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDestinations()
        {
            try
            {
                var result = await _destinationService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDestination(string id)
        {
            try
            {
                var destination = await _destinationService.GetByIdAsync(id);
                if (destination == null) return NotFound();
                return Ok(destination);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDestination([FromForm] DestinationCreateDto dto)
        {
            string imageUrl = null;
            try
            {
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);
                }
                
                var createdId = await _destinationService.CreateAsync(dto, imageUrl);
                return CreatedAtAction(nameof(GetDestination), new { id = createdId }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDestination(string id, [FromForm] DestinationCreateDto dto)
        {
            string imageUrl = null;
            try
            {

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);
                }
                await _destinationService.UpdateAsync(id, dto,imageUrl);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(string id)
        {
            try
            {
                var success = await _destinationService.RemoveAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDestinations([FromQuery] string name)
        {
            try
            {
                var result = await _destinationService.SearchDestinationsByNameAsync(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
