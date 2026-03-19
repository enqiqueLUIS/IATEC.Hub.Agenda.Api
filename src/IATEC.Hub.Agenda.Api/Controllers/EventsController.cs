using Microsoft.AspNetCore.Mvc;
using IATEC.Hub.Agenda.Api.DTOs;
using IATEC.Hub.Agenda.Api.Services;

namespace IATEC.Hub.Agenda.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _service;

    public EventsController(IEventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EventFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        var (result, error) = await _service.CreateAsync(dto);
        if (error is not null) return BadRequest(new { error });
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
    {
        var (result, error) = await _service.UpdateAsync(id, dto);
        if (error == "Event not found.") return NotFound();
        if (error is not null) return BadRequest(new { error });
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/share")]
    public async Task<IActionResult> Share(int id, [FromBody] ShareEventDto dto)
    {
        var (success, error) = await _service.ShareAsync(id, dto);
        if (error == "Event not found.") return NotFound();
        if (error is not null) return BadRequest(new { error });
        return NoContent();
    }
}
