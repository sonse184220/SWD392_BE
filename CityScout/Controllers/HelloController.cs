using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHello()
    {
        return Ok("Hello may ban test api ne hihi!");
    }
}
