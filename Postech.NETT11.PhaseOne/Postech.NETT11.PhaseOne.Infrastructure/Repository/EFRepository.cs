using Microsoft.EntityFrameworkCore;
using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.Repositories;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository;

public class EFRepository<T>:IRepository<T> where T: BaseEntity
{
    protected AppDbContext _context;
    protected DbSet<T> _dbSet;

    public EFRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public virtual async Task<T?> GetByIdAsync(Guid id) 
        => await _dbSet.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var result = _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        var result = _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        if (await GetByIdAsync(id) is not T entity)
            return false;
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;

    }
}