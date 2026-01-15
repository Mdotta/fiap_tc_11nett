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
        
        builder.Property(x => x.ReleaseDate)
            .HasColumnType("DATETIME2")
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

        builder.Property(x => x.Categories)
            .HasColumnType("NVARCHAR(100)")
            .HasConversion(
                v => v == null || !v.Any() ? string.Empty : string.Join(",", v.Select(c => c.ToString())),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<GameCategory>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => Enum.Parse<GameCategory>(c))
                        .ToList())
            .IsRequired(false)
            .Metadata.SetValueComparer(
                new ValueComparer<List<GameCategory>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

    }
}