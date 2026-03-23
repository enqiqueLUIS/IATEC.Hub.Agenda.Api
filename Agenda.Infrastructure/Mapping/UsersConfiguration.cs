using Agenda.Core.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Mapping;

public class UsersConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> entity)
    {
        entity.ToTable("Users");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .HasColumnName("Id")
              .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
              .IsRequired()
              .HasMaxLength(100)
              .HasColumnName("Name");

        entity.Property(e => e.Email)
              .IsRequired()
              .HasMaxLength(150)
              .HasColumnName("Email");

        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.Password)
              .IsRequired()
              .HasMaxLength(255)
              .HasColumnName("Password");

        entity.HasData(
            new Users { Id = 1, Name = "Admin",       Email = "admin@agenda.com", Password = "admin123" },
            new Users { Id = 2, Name = "Luis García", Email = "luis@agenda.com",  Password = "luis123"  },
            new Users { Id = 3, Name = "María López", Email = "maria@agenda.com", Password = "maria123" }
        );
    }
}