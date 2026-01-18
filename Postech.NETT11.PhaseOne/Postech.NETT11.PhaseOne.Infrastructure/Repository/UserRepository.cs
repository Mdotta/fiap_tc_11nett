using Microsoft.EntityFrameworkCore;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
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
        return await _dbSet.AsNoTracking()
            .Where(x => x.IsActive == true)
            .ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Id == id && x.IsActive == true)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByIdIncludingInactiveAsync(Guid id)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
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
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username && x.IsActive);
    }

    public async Task<User?> GetByUsernameIncludingInactiveAsync(string username)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null)
    {
        var query = _dbSet.AsNoTracking().Where(x => x.Username == username);
        
        if (excludeUserId.HasValue)
            query = query.Where(x => x.Id != excludeUserId.Value);
        
        return await query.AnyAsync();
    }

    public async Task<bool> UserHandleExistsAsync(string userHandle, Guid? excludeUserId = null)
    {
        var query = _dbSet.AsNoTracking().Where(x => x.UserHandle == userHandle);
        
        if (excludeUserId.HasValue)
            query = query.Where(x => x.Id != excludeUserId.Value);
        
        return await query.AnyAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null)
    {
        var query = _dbSet.AsNoTracking().Where(x => x.Email == email);
        
        if (excludeUserId.HasValue)
            query = query.Where(x => x.Id != excludeUserId.Value);
        
        return await query.AnyAsync();
    }

    public async Task<int> CountAdminsAsync()
    {
        return await _dbSet.AsNoTracking()
            .CountAsync(x => x.Role == UserRole.Admin && x.IsActive);
    }

    public async Task<bool> ReactivateUserAsync(Guid id)
    {
        var user = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return false;

        if (user.IsActive)
            return false;

        user.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }
}