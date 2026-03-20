using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Context.SQLServer;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder){}
}