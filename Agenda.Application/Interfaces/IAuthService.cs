using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.QueryFilters;

namespace Agenda.Application.Interfaces;

public interface IAuthService
{
    Task<ResponseGetObject> Login(LoginQueryFilter queryFilter);
}