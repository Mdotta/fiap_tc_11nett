using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.Repositories;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository;

public class UserRepository:EFRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public User? GetByCredentials(string username, string passwordHash)
    {
        return _dbSet.FirstOrDefault(x=>x.Username == username && x.PasswordHash == passwordHash);
    }
}