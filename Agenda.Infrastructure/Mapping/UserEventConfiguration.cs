using Agenda.Core.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Mapping;

public class UserEventConfiguration : IEntityTypeConfiguration<UserEvent>
{
    public void Configure(EntityTypeBuilder<UserEvent> entity)
    {
        entity.ToTable("UserEvents");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .HasColumnName("Id")
              .ValueGeneratedOnAdd();

        entity.Property(e => e.UserId)
              .IsRequired()
              .HasColumnName("UserId");

        entity.Property(e => e.EventId)
              .IsRequired()
              .HasColumnName("EventId");

        entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Active")
              .HasColumnName("Status");

        entity.HasData(
            new UserEvent { Id = 1, UserId = 2, EventId = 1, Status = "Active" },
            new UserEvent { Id = 2, UserId = 2, EventId = 2, Status = "Active" },
            new UserEvent { Id = 3, UserId = 3, EventId = 3, Status = "Active" }
        );
    }
}