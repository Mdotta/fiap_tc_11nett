using Microsoft.EntityFrameworkCore;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.Common;
using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.Repositories;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository;

public class UserRepository:EFRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
    
    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        var query = _dbSet.AsQueryable().AsNoTracking();
        query = query.Where(x => x.IsActive == true);
        return await query.ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        var query = _dbSet.AsQueryable().AsNoTracking();
        query = query.Where(x => x.IsActive == true);
        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return false;

        if (!user.IsActive)
            throw new DomainException("User is already deleted");

        user.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetByUsername(string username)
    {
        var user = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
        return user;
    }

    public async Task<bool> UsernameExistsAsync(string username,Guid? excludeUserId = null)
    {
        return await _dbSet.AsNoTracking().AnyAsync(x=>x.Username==username && x.IsActive && x.Id != excludeUserId);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null)
    {
        return await _dbSet.AsNoTracking().AnyAsync(x => x.Email == email && x.IsActive && x.Id != excludeUserId);
    }
}