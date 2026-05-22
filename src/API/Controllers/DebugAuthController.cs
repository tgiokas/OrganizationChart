using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationImport.Api.Controllers;

[ApiController]
[Route("debug/auth")]
public class DebugAuthController : ControllerBase
{
    [HttpGet("claims")]
    [Authorize]
    public IActionResult GetClaims()
    {
        var claims = User.Claims
            .Select(c => new { c.Type, c.Value })
            .ToList();
        
        return Ok(claims);
    }
}