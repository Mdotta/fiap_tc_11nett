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
    public IList<T> GetAll()
        => _dbSet.ToList();

    public T? GetById(Guid id) 
        => _dbSet.FirstOrDefault(x => x.Id == id);

    public void Add(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        _dbSet.Add(entity);
        _context.SaveChanges();
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        if(GetById(id) is not T entity)
            return;
        _dbSet.Remove(entity);
        _context.SaveChanges();
    }
}