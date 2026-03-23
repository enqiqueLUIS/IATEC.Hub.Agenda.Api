using System.Net;
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

            var currentEvents = allEvents
                .Where(e => e.StartDate <= now && e.EndDate >= now)
                .OrderBy(e => e.EndDate)
                .ToList();

            var upcomingEvents = allEvents
                .Where(e => e.StartDate > now)
                .OrderBy(e => e.StartDate)
                .ToList();

            return new ResponseGetObject
            {
                Data = new
                {
                    CurrentEvents = currentEvents,
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