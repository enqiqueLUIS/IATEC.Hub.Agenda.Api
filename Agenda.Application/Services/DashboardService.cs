using System.Net;
using Agenda.Application.Dtos;
using Agenda.Application.Interfaces;
using Agenda.Core.Entities.Core.CustomEntities.ResponseApi.Details;
using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.Interfaces;

namespace Agenda.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseGetObject> GetDashboard(int userId)
    {
        try
        {
            var now = DateTime.UtcNow;

            var userEvents = await _unitOfWork.UserEventsRepository
                .Find(ue => ue.UserId == userId && ue.Status == "Active");

            var eventIds = userEvents.Select(ue => ue.EventId).ToList();

            var allEvents = await _unitOfWork.EventsRepository
                .Find(e => eventIds.Contains(e.Id) && e.Status == 1);

            var ongoingRaw = allEvents
                .Where(e => e.StartDate <= now && e.EndDate >= now)
                .OrderBy(e => e.EndDate)
                .ToList();

            var upcomingRaw = allEvents
                .Where(e => e.StartDate > now)
                .OrderBy(e => e.StartDate)
                .ToList();

            var allUserIds = ongoingRaw.Select(e => e.CreatedBy)
                .Concat(upcomingRaw.Select(e => e.CreatedBy))
                .Distinct().ToList();

            var users = new Dictionary<int, string>();
            foreach (var uid in allUserIds)
            {
                var u = await _unitOfWork.UsersRepository.GetByIdAsync(uid);
                users[uid] = u?.Name ?? "Desconocido";
            }

            DashboardEventDto Map(Agenda.Core.Entities.Core.Event e) => new()
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                EventType = e.EventType,
                CreatorName = users.GetValueOrDefault(e.CreatedBy, "Desconocido")
            };

            var ongoingEvents  = ongoingRaw.Select(Map).ToList();
            var upcomingEvents = upcomingRaw.Select(Map).ToList();

            return new ResponseGetObject
            {
                Data = new
                {
                    OngoingEvents = ongoingEvents,
                    UpcomingEvents = upcomingEvents
                },
                Messages = new[] { new Message { Type = "success", Description = "Dashboard obtenido correctamente." } },
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception)
        {
            return new ResponseGetObject
            {
                Data = null,
                Messages = new[] { new Message { Type = "error", Description = "Error al obtener el dashboard." } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}