using Agenda.Core.Entities.Core.ResponseApi;

namespace Agenda.Application.Interfaces;

public interface IDashboardService
{
    Task<ResponseGetObject> GetDashboard(int userId);
}