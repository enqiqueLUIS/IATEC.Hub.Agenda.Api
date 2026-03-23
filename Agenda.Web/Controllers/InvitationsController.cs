using System.Security.Claims;
using Agenda.Application.Interfaces;
using Agenda.Core.QueryFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Web.Controllers;

[ApiController]
[Route("api/invitations")]
[Authorize]
public class InvitationsController : ControllerBase
{
    private readonly IInvitationsService _invitationsService;

    public InvitationsController(IInvitationsService invitationsService)
    {
        _invitationsService = invitationsService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue("userId")!);

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _invitationsService.GetPendingInvitations(GetUserId());
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] EventInvitationsQueryFilter queryFilter)
    {
        var result = await _invitationsService.SendInvitation(GetUserId(), queryFilter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("{id}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        var result = await _invitationsService.AcceptInvitation(GetUserId(), id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await _invitationsService.RejectInvitation(GetUserId(), id);
        return StatusCode((int)result.StatusCode, result);
    }
}