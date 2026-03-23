using Agenda.Core.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Mapping;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> entity)
    {
        entity.ToTable("Events");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .HasColumnName("Id")
              .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedBy)
              .IsRequired()
              .HasColumnName("CreatedBy");

        entity.Property(e => e.Title)
              .IsRequired()
              .HasMaxLength(200)
              .HasColumnName("Title");

        entity.Property(e => e.Description)
              .HasMaxLength(1000)
              .HasColumnName("Description");

        entity.Property(e => e.StartDate)
              .IsRequired()
              .HasColumnType("datetime2(0)")
              .HasColumnName("StartDate");

        entity.Property(e => e.EndDate)
              .IsRequired()
              .HasColumnType("datetime2(0)")
              .HasColumnName("EndDate");

        entity.Property(e => e.Location)
              .HasMaxLength(300)
              .HasColumnName("Location");

        entity.Property(e => e.EventType)
              .IsRequired()
              .HasMaxLength(20)
              .HasColumnName("EventType");

        entity.Property(e => e.Status)
              .IsRequired()
              .HasDefaultValue(1)
              .HasColumnName("Status");

        entity.HasData(
            new Event
            {
                Id = 1, CreatedBy = 2, Title = "Reunión de equipo",
                Description = "Reunión semanal del equipo de desarrollo",
                StartDate = new DateTime(2026, 3, 25, 9, 0, 0),
                EndDate   = new DateTime(2026, 3, 25, 10, 0, 0),
                Location  = "Sala A", EventType = "Shared", Status = 1
            },
            new Event
            {
                Id = 2, CreatedBy = 2, Title = "Revisión de proyecto",
                Description = "Revisión del avance del proyecto IATEC",
                StartDate = new DateTime(2026, 3, 26, 14, 0, 0),
                EndDate   = new DateTime(2026, 3, 26, 15, 30, 0),
                Location  = "Sala B", EventType = "Exclusive", Status = 1
            },
            new Event
            {
                Id = 3, CreatedBy = 3, Title = "Capacitación",
                Description = "Capacitación en nuevas tecnologías",
                StartDate = new DateTime(2026, 3, 27, 10, 0, 0),
                EndDate   = new DateTime(2026, 3, 27, 12, 0, 0),
                Location  = "Auditorio", EventType = "Shared", Status = 1
            }
        );
    }
}