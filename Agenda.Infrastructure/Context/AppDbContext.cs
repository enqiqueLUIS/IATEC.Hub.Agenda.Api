using Agenda.Core.Entities.Core;
using Agenda.Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Context.SQLServer;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Users>            Users             { get; set; }
    public DbSet<Event>            Events            { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    public DbSet<EventInvitation>  EventInvitations  { get; set; }
    public DbSet<UserEvent>        UserEvents        { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersConfiguration).Assembly);
    }
}