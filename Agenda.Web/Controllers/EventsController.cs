using System.Security.Claims;
using Agenda.Application.Interfaces;
using Agenda.Core.QueryFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Web.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventsService _eventsService;

    public EventsController(IEventsService eventsService)
    {
        _eventsService = eventsService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue("userId")!);

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] EventsQueryFilter queryFilter)
    {
        var result = await _eventsService.GetEvents(GetUserId(), queryFilter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] EventsQueryFilter queryFilter)
    {
        var result = await _eventsService.InsertEvent(GetUserId(), queryFilter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EventsQueryFilter queryFilter)
    {
        var result = await _eventsService.UpdateEvent(GetUserId(), queryFilter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _eventsService.DeleteEvent(GetUserId(), id);
        return StatusCode((int)result.StatusCode, result);
    }
}