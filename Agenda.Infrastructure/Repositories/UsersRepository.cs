using Agenda.Core.Entities.Core;
using Agenda.Core.Interfaces;
using Agenda.Core.QueryFilters;
using Agenda.Infrastructure.Context.SQLServer;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _context;

    public UsersRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UsersQueryFilter>> GetAllUsers()
    {
        return await _context.Set<Users>()
            .Select(u => new UsersQueryFilter
            {
                Id       = u.Id,
                Name     = u.Name,
                Email    = u.Email,
                Password = u.Password
            })
            .ToListAsync();
    }
}