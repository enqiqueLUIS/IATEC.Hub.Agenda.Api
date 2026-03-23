using Agenda.Core.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Mapping;

public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
{
    public void Configure(EntityTypeBuilder<EventParticipant> entity)
    {
        entity.ToTable("EventParticipants");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .HasColumnName("Id")
              .ValueGeneratedOnAdd();

        entity.Property(e => e.EventId)
              .IsRequired()
              .HasColumnName("EventId");

        entity.Property(e => e.UserId)
              .IsRequired()
              .HasColumnName("UserId");
    }
}