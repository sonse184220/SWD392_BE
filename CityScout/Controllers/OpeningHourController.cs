using CityScout.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace CityScout.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpeningHourController : ControllerBase
    {
        private readonly IOpeningHourService _openingHourService;

        public OpeningHourController(IOpeningHourService openingHourService)
        {
            _openingHourService = openingHourService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _openingHourService.GetAllAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpGet("{destinationId}")]
        public async Task<IActionResult> GetByDestinationId(string destinationId)
        {
            try
            {
                var result = await _openingHourService.GetByDestinationIdAsync(destinationId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OpeningHourDto dto)
        {
            try
            {
                var result = await _openingHourService.CreateAsync(dto);
                if (!result) return BadRequest("Failed to create opening hour");
                return Ok("Opening hour created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OpeningHourDto dto)
        {
            try
            {
                var result = await _openingHourService.UpdateAsync(dto);
                if (!result) return NotFound("Opening hour not found");
                return Ok("Opening hour updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("{destinationId}/{dayOfWeek}")]
        public async Task<IActionResult> Delete(string destinationId, string dayOfWeek)
        {
            try
            {
                var result = await _openingHourService.DeleteAsync(destinationId, dayOfWeek);
                if (!result) return NotFound("Opening hour not found");
                return Ok("Opening hour deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
