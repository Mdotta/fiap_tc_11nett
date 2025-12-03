using Microsoft.EntityFrameworkCore;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository;

public class AppDbContext :DbContext
{
    private string _connectionString;
    public AppDbContext()
    {
        
    }

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}