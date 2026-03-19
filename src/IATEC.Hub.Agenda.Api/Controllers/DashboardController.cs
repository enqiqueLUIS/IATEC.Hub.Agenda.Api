using Microsoft.AspNetCore.Mvc;
using IATEC.Hub.Agenda.Api.Services;

namespace IATEC.Hub.Agenda.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IEventService _service;

    public DashboardController(IEventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? user)
    {
        var result = await _service.GetDashboardAsync(user);
        return Ok(result);
    }
}
