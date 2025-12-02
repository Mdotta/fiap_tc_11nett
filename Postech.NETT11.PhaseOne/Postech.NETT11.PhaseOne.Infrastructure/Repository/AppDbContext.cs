using Microsoft.EntityFrameworkCore;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository;

public class AppDbContext:DbContext
{
    private readonly string _connectionString;

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}