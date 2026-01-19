using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.Repositories;

namespace Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;

public interface IUserRepository:IRepository<User>
{
    Task<User?> GetByUsername(string username);
    
    Task<User?> GetByUsernameIncludingInactiveAsync(string username);
    
    Task<User?> GetByIdIncludingInactiveAsync(Guid id);
    
    Task<bool> UsernameExistsAsync(string username,Guid? excludeUserId = null);
    
    Task<bool> UserHandleExistsAsync(string userHandle, Guid? excludeUserId = null);
    
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
    
    Task<int> CountAdminsAsync();
    
    Task<bool> ReactivateUserAsync(Guid id);
}