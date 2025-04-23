using System.Collections.Concurrent;
using game.Models;
using Microsoft.AspNetCore.Mvc;

namespace game.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
   

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("ok");
    }
}