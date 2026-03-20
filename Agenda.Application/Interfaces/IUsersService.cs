using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.QueryFilters;

namespace Agenda.Application;

public interface IUsersService
{
    Task<object> GetAllUsers(PaginationQueryFilter queryFilter);
    Task<ResponseGetObject> GetUserById(int id);
    Task<ResponsePost> InsertUsers(UsersQueryFilter queryFilter);
    Task<ResponsePost> UpdateUsers(UsersQueryFilter queryFilter);
    Task<ResponsePost> DeleteUsers(UsersQueryFilter queryFilter);
    
}