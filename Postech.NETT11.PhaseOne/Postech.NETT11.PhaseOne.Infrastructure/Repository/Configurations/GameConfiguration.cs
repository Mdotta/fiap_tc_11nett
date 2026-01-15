using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository.Configurations;

public class GameConfiguration:IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Game");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnType("UNIQUEIDENTIFIER")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Title)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasColumnType("NVARCHAR(500)")
            .IsRequired();
        
        builder.Property(x => x.Developer)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();
        
        builder.Property(x => x.Publisher)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();
        
        builder.Property(x => x.Price)
            .HasColumnType("DECIMAL(18,2)")
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
        
        builder.Property(x => x.Status)
            .HasColumnType("NVARCHAR(20)")
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<GameStatus>(v))
            .IsRequired();
    }
}