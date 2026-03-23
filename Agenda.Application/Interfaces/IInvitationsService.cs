using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.QueryFilters;

namespace Agenda.Application.Interfaces;

public interface IInvitationsService
{
    Task<ResponseGetObject> GetPendingInvitations(int userId);
    Task<ResponsePost> SendInvitation(int userId, EventInvitationsQueryFilter queryFilter);
    Task<ResponsePost> AcceptInvitation(int userId, int invitationId);
    Task<ResponsePost> RejectInvitation(int userId, int invitationId);
}