using System.Net;
using Agenda.Application.Interfaces;
using Agenda.Core.Entities.Core;
using Agenda.Core.Entities.Core.CustomEntities.ResponseApi.Details;
using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.Interfaces;
using Agenda.Core.QueryFilters;
using FluentValidation;

namespace Agenda.Application.Services;

public class EventsService : IEventsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<EventsQueryFilter> _validator;

    public EventsService(IUnitOfWork unitOfWork, IValidator<EventsQueryFilter> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ResponseGetObject> GetEvents(int userId, EventsQueryFilter queryFilter)
    {
        try
        {
            var userEvents = await _unitOfWork.UserEventsRepository
                .Find(ue => ue.UserId == userId && ue.Status == "Active");

            var eventIds = userEvents.Select(ue => ue.EventId).ToList();

            var events = await _unitOfWork.EventsRepository
                .Find(e => eventIds.Contains(e.Id) && e.Status == 1);

            if (queryFilter.FilterDate.HasValue)
            {
                var date = queryFilter.FilterDate.Value.Date;
                events = events.Where(e => e.StartDate.Date <= date && e.EndDate.Date >= date);
            }

            if (!string.IsNullOrWhiteSpace(queryFilter.SearchCriteria))
            {
                var criteria = queryFilter.SearchCriteria.ToLower();
                events = events.Where(e =>
                    e.Title.ToLower().Contains(criteria) ||
                    (e.Description != null && e.Description.ToLower().Contains(criteria)) ||
                    (e.Location != null && e.Location.ToLower().Contains(criteria)));
            }

            var result = events.OrderBy(e => e.StartDate).ToList();

            return new ResponseGetObject
            {
                Data = result,
                Messages = new[] { new Message { Type = "success", Description = "Eventos obtenidos correctamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponseGetObject
            {
                Data = null,
                Messages = new[] { new Message { Type = "error", Description = "Error al obtener los eventos." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> InsertEvent(int userId, EventsQueryFilter queryFilter)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(queryFilter);
            if (!validationResult.IsValid)
            {
                return new ResponsePost
                {
                    Messages = validationResult.Errors.Select(e => new Message { Type = "error", Description = e.ErrorMessage }).ToArray(),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if (queryFilter.EventType == "Exclusive")
            {
                var overlap = await HasExclusiveOverlap(userId, queryFilter.StartDate, queryFilter.EndDate);
                if (overlap)
                {
                    return new ResponsePost
                    {
                        Messages = new[] { new Message { Type = "error", Description = "Ya existe un evento exclusivo en ese horario." } },
                        StatusCode = HttpStatusCode.Conflict
                    };
                }
            }

            var newEvent = new Event
            {
                CreatedBy = userId,
                Title = queryFilter.Title,
                Description = queryFilter.Description,
                StartDate = queryFilter.StartDate,
                EndDate = queryFilter.EndDate,
                Location = queryFilter.Location,
                EventType = queryFilter.EventType,
                Status = 1
            };

            var result = await _unitOfWork.EventsRepository.InsertAsync(newEvent);

            await _unitOfWork.UserEventsRepository.InsertAsync(new UserEvent
            {
                UserId = userId,
                EventId = result.IdGenerated,
                Status = "Active"
            });

            if (queryFilter.Participants != null && queryFilter.Participants.Any())
            {
                var participants = queryFilter.Participants.Select(p => new EventParticipant
                {
                    EventId = result.IdGenerated,
                    UserId = p
                }).ToList();

                await _unitOfWork.EventParticipantsRepository.InsertAsync(participants);
            }

            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "success", Description = "Evento creado exitosamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al crear el evento." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> UpdateEvent(int userId, EventsQueryFilter queryFilter)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(queryFilter);
            if (!validationResult.IsValid)
            {
                return new ResponsePost
                {
                    Messages = validationResult.Errors.Select(e => new Message { Type = "error", Description = e.ErrorMessage }).ToArray(),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if (queryFilter.Id <= 0)
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "El Id del evento no es válido." } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var existing = await _unitOfWork.EventsRepository.GetByIdAsync(queryFilter.Id);
            if (existing == null || existing.CreatedBy != userId)
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "Evento no encontrado o sin permisos para editarlo." } },
                    StatusCode = HttpStatusCode.Forbidden
                };
            }

            if (queryFilter.EventType == "Exclusive")
            {
                var overlap = await HasExclusiveOverlap(userId, queryFilter.StartDate, queryFilter.EndDate, queryFilter.Id);
                if (overlap)
                {
                    return new ResponsePost
                    {
                        Messages = new[] { new Message { Type = "error", Description = "Ya existe un evento exclusivo en ese horario." } },
                        StatusCode = HttpStatusCode.Conflict
                    };
                }
            }

            _unitOfWork.EventsRepository.UpdateCustom(
                new Event
                {
                    Id = queryFilter.Id,
                    CreatedBy = userId,
                    Title = queryFilter.Title,
                    Description = queryFilter.Description,
                    StartDate = queryFilter.StartDate,
                    EndDate = queryFilter.EndDate,
                    Location = queryFilter.Location,
                    EventType = queryFilter.EventType
                },
                x => x.Title,
                x => x.Description,
                x => x.StartDate,
                x => x.EndDate,
                x => x.Location,
                x => x.EventType
            );

            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "success", Description = "Evento actualizado exitosamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al actualizar el evento." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponsePost> DeleteEvent(int userId, int eventId)
    {
        try
        {
            var existing = await _unitOfWork.EventsRepository.GetByIdAsync(eventId);
            if (existing == null || existing.CreatedBy != userId)
            {
                return new ResponsePost
                {
                    Messages = new[] { new Message { Type = "error", Description = "Evento no encontrado o sin permisos para eliminarlo." } },
                    StatusCode = HttpStatusCode.Forbidden
                };
            }

            _unitOfWork.EventsRepository.UpdateCustom(
                new Event { Id = eventId, Status = 0 },
                x => x.Status
            );

            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "success", Description = "Evento eliminado exitosamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponsePost
            {
                Messages = new[] { new Message { Type = "error", Description = "Error al eliminar el evento." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    private async Task<bool> HasExclusiveOverlap(int userId, DateTime startDate, DateTime endDate, int excludeEventId = 0)
    {
        var userEvents = await _unitOfWork.UserEventsRepository
            .Find(ue => ue.UserId == userId && ue.Status == "Active");

        var eventIds = userEvents.Select(ue => ue.EventId).ToList();

        var exclusiveEvents = await _unitOfWork.EventsRepository
            .Find(e => eventIds.Contains(e.Id)
                    && e.EventType == "Exclusive"
                    && e.Status == 1
                    && e.Id != excludeEventId
                    && e.StartDate < endDate
                    && e.EndDate > startDate);

        return exclusiveEvents.Any();
    }
}