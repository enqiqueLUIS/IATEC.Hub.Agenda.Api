using Agenda.Application.Interfaces;
using Agenda.Core.QueryFilters;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginQueryFilter queryFilter)
    {
        var result = await _authService.Login(queryFilter);
        return StatusCode((int)result.StatusCode, result);
    }
}