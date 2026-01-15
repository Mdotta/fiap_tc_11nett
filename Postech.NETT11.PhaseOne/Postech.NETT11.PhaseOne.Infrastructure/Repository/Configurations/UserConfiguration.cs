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

        builder.Property(x => x.IsActive)
            .HasColumnType("BIT")
            .HasDefaultValue(true)
            .IsRequired();

        AddStartingData(builder);
    }

    private void AddStartingData(EntityTypeBuilder<User> builder)
    {
        var userAdmin = new User()
        {
            Id =  new Guid("6C511C9F-CAE7-4D08-8101-F8AF7C81357A"),
            CreatedAt = new DateTime(year:2025, month:1, day:1),
            UserHandle = "admin",
            Username = "admin",
            PasswordHash = "tempPass",
            Role = UserRole.Admin,
            IsActive = true
        };
        
        var userClient = new User()
        {
            Id = new Guid("4D9C6BD6-821A-40F0-B2B9-64683B5E91E1"),
            CreatedAt = new DateTime(year:2025, month:1, day:1),
            UserHandle = "client",
            Username = "client",
            PasswordHash = "tempPass",
            Role = UserRole.Client,
            IsActive = true
        };
        
        builder.HasData(userAdmin, userClient);
    }
}