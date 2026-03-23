using Agenda.Core.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Mapping;

public class EventInvitationConfiguration : IEntityTypeConfiguration<EventInvitation>
{
    public void Configure(EntityTypeBuilder<EventInvitation> entity)
    {
        entity.ToTable("EventInvitations");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .HasColumnName("Id")
              .ValueGeneratedOnAdd();

        entity.Property(e => e.EventId)
              .IsRequired()
              .HasColumnName("EventId");

        entity.Property(e => e.InvitedByUserId)
              .IsRequired()
              .HasColumnName("InvitedByUserId");

        entity.Property(e => e.InvitedUserId)
              .IsRequired()
              .HasColumnName("InvitedUserId");

        entity.Property(e => e.SentAt)
              .IsRequired()
              .HasColumnType("datetime2(0)")
              .HasColumnName("SentAt");

        entity.Property(e => e.RespondedAt)
              .HasColumnType("datetime2(0)")
              .HasColumnName("RespondedAt");

        entity.Property(e => e.Status)
              .IsRequired()
              .HasMaxLength(20)
              .HasDefaultValue("Pending")
              .HasColumnName("Status");
    }
}