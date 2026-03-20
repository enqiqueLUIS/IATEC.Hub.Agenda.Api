using Agenda.Core.QueryFilters;

namespace Agenda.Core.Interfaces;

public interface IUsersRepository
{
    Task<IEnumerable<UsersQueryFilter>> GetAllUsers();
}