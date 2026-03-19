using Microsoft.EntityFrameworkCore;
using IATEC.Hub.Agenda.Api.Models;

namespace IATEC.Hub.Agenda.Api.Data;

public class AgendaDbContext : DbContext
{
    public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventShare> EventShares => Set<EventShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(300);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<EventShare>(entity =>
        {
            entity.HasKey(es => es.Id);
            entity.HasOne(es => es.Event)
                  .WithMany(e => e.Shares)
                  .HasForeignKey(es => es.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(es => es.SharedWith).IsRequired().HasMaxLength(100);
        });
    }
}
