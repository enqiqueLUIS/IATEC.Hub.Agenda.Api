using System.Net;
using Agenda.Application.Interfaces;
using Agenda.Core.Entities.Core;
using Agenda.Core.Entities.Core.CustomEntities.ResponseApi.Details;
using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.Interfaces;
using Agenda.Core.QueryFilters;

namespace Agenda.Application.Services;

public class InvitationsService : IInvitationsService
{
    private readonly IUnitOfWork _unitOfWork;

    public InvitationsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseGetObject> GetPendingInvitations(int userId)
    {
        try
        {
            var invitations = await _unitOfWork.EventInvitationsRepository
                .Find(i => i.InvitedUserId == userId && i.Status == "Pending");

            return new ResponseGetObject
            {
                Data = invitations,
                Messages = new[] { new Message { Type = "success", Description = "Invitaciones obtenidas correctamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponseGetObject
            {
                Data = null,
                Messages = new[] { new Message { Type = "error", Description = "Error al obtener las invitaciones." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> SendInvitation(int userId, EventInvitationsQueryFilter queryFilter)
    {
        try
        {
            var eventExists = await _unitOfWork.EventsRepository.GetByIdAsync(queryFilter.EventId);
            if (eventExists == null || eventExists.Status == 0)
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "El evento no existe." } },
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            var userExists = await _unitOfWork.UsersRepository.GetByIdAsync(queryFilter.InvitedUserId);
            if (userExists == null)
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "El usuario invitado no existe." } },
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            var alreadyInAgenda = await _unitOfWork.UserEventsRepository
                .Find(ue => ue.UserId == queryFilter.InvitedUserId && ue.EventId == queryFilter.EventId);
            if (alreadyInAgenda.Any())
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "El usuario ya tiene este evento en su agenda." } },
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            var alreadyInvited = await _unitOfWork.EventInvitationsRepository
                .Find(i => i.EventId == queryFilter.EventId
                        && i.InvitedUserId == queryFilter.InvitedUserId
                        && i.Status == "Pending");
            if (alreadyInvited.Any())
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "Ya existe una invitación pendiente para este usuario." } },
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            await _unitOfWork.EventInvitationsRepository.InsertAsync(new EventInvitation
            {
                EventId = queryFilter.EventId,
                InvitedByUserId = userId,
                InvitedUserId = queryFilter.InvitedUserId,
                SentAt = DateTime.UtcNow,
                Status = "Pending"
            });

            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "success", Description = "Invitación enviada exitosamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al enviar la invitación." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> AcceptInvitation(int userId, int invitationId)
    {
        try
        {
            var invitation = await _unitOfWork.EventInvitationsRepository.GetByIdAsync(invitationId);
            if (invitation == null || invitation.InvitedUserId != userId || invitation.Status != "Pending")
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "Invitación no encontrada o no disponible." } },
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            _unitOfWork.EventInvitationsRepository.UpdateCustom(
                new EventInvitation { Id = invitationId, Status = "Accepted", RespondedAt = DateTime.UtcNow },
                x => x.Status,
                x => x.RespondedAt
            );

            await _unitOfWork.UserEventsRepository.InsertAsync(new UserEvent
            {
                UserId = userId,
                EventId = invitation.EventId,
                Status = "Active"
            });

            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "success", Description = "Invitación aceptada. El evento fue agregado a tu agenda." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al aceptar la invitación." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> RejectInvitation(int userId, int invitationId)
    {
        try
        {
            var invitation = await _unitOfWork.EventInvitationsRepository.GetByIdAsync(invitationId);
            if (invitation == null || invitation.InvitedUserId != userId || invitation.Status != "Pending")
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "Invitación no encontrada o no disponible." } },
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            _unitOfWork.EventInvitationsRepository.UpdateCustom(
                new EventInvitation { Id = invitationId, Status = "Rejected", RespondedAt = DateTime.UtcNow },
                x => x.Status,
                x => x.RespondedAt
            );

            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "success", Description = "Invitación rechazada." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al rechazar la invitación." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}
