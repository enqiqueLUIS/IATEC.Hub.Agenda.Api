using Agenda.Application;
using Agenda.Application.Interfaces;
using Agenda.Core.QueryFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Web.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryFilter queryFilter)
    {
        var result = await _usersService.GetAllUsers(queryFilter);
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Insert([FromBody] UsersQueryFilter queryFilter)
    {
        var result = await _usersService.InsertUsers(queryFilter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UsersQueryFilter queryFilter)
    {
        var result = await _usersService.UpdateUsers(queryFilter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _usersService.DeleteUsers(new UsersQueryFilter { Id = id });
        return StatusCode((int)result.StatusCode, result);
    }
}