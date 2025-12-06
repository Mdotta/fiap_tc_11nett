using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.Repositories;

namespace Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;

public interface IUserRepository:IRepository<User>
{
    User? GetByCredentials(string username, string passwordHash);
}