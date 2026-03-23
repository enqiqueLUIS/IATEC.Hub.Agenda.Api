using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.QueryFilters;

namespace Agenda.Application.Interfaces;

public interface IEventsService
{
    Task<ResponseGetObject> GetEvents(int userId, EventsQueryFilter queryFilter);
    Task<ResponsePost> InsertEvent(int userId, EventsQueryFilter queryFilter);
    Task<ResponsePost> UpdateEvent(int userId, EventsQueryFilter queryFilter);
    Task<ResponsePost> DeleteEvent(int userId, int eventId);
}