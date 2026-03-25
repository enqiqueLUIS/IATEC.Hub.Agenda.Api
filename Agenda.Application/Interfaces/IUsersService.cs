using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.QueryFilters;

namespace Agenda.Application.Interfaces;

public interface IUsersService
{
    Task<object> GetAllUsers(PaginationQueryFilter queryFilter);
    Task<ResponsePost> InsertUsers(UsersQueryFilter queryFilter);
    Task<ResponsePost> UpdateUsers(UsersQueryFilter queryFilter);
    Task<ResponsePost> DeleteUsers(UsersQueryFilter queryFilter);
    
}