using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityScout.Controllers
{
    [Route("CityScout/destinations")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService _service;

        public DestinationController(IDestinationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Destination>>> GetAll()
            => Ok(await _service.GetAllDestinationsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Destination>> GetById(int id)
        {
            var result = await _service.GetDestinationByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Destination>>> Search([FromQuery] string query)
            => Ok(await _service.SearchDestinationsAsync(query));

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Destination destination)
        {
            await _service.AddDestinationAsync(destination);
            return CreatedAtAction(nameof(GetById), new { id = destination.DestinationId }, destination);
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Destination destination)
        {
            destination.DestinationId = id;
            await _service.UpdateDestinationAsync(destination);
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteDestinationAsync(id);
            return NoContent();
        }
    }
}
