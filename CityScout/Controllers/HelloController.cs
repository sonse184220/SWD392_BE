using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

[Route("api/[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    private readonly CityScoutContext _context;

    public HelloController(CityScoutContext context)
    {
        _context = context;
    }
    [HttpGet("hola")]
    public IActionResult GetHello()
    {
        return Ok("Hello may ban test api ne hihi!");
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        return await _context.Roles.ToListAsync();
    }
}
