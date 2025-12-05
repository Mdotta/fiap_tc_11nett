using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Domain.Entities;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository.Configurations;

public class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnType("UNIQUEIDENTIFIER")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAt)
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
        
        builder.Property(x => x.UserHandle)
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(x => x.Username)
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnType("NVARCHAR(10)")
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<UserRole>(v))
            .IsRequired();

        AddStartingData(builder);
    }

    private void AddStartingData(EntityTypeBuilder<User> builder)
    {
        var userAdmin = new User()
        {
            Id = Guid.NewGuid(),
            CreatedAt = new DateTime(year:2025, month:1, day:1),
            UserHandle = "admin",
            Username = "admin",
            PasswordHash = "tempPass",
            Role = UserRole.Admin
        };
        
        var userClient = new User()
        {
            Id = Guid.NewGuid(),
            CreatedAt = new DateTime(year:2025, month:1, day:1),
            UserHandle = "client",
            Username = "client",
            PasswordHash = "tempPass",
            Role = UserRole.Client
        };
        
        builder.HasData(userAdmin, userClient);
    }
}