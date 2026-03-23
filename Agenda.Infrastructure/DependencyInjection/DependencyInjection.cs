using Agenda.Application.Interfaces;
using Agenda.Application.Services;
using Agenda.Application.Validators;
using Agenda.Core.Interfaces;
using Agenda.Core.QueryFilters;
using Agenda.Infrastructure.Context.SQLServer;
using Agenda.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agenda.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Missing connection string 'DefaultConnection' in appsettings.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IEventsService, EventsService>();
        services.AddScoped<IInvitationsService, InvitationsService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<IValidator<UsersQueryFilter>, UsersValidator>();
        services.AddScoped<IValidator<EventsQueryFilter>, EventsValidator>();

        return services;
    }
}