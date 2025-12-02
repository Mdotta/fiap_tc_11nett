using Microsoft.EntityFrameworkCore;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository;

public class AppDbContext(DbContextOptions options):DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}