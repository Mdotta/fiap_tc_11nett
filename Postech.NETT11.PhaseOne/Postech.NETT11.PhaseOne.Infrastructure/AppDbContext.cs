using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Postech.NETT11.PhaseOne.Infrastructure;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}