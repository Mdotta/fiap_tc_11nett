using Postech.NETT11.PhaseOne.Domain.Entities;

namespace Postech.NETT11.PhaseOne.Domain.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IList<T> GetAll();
    T? GetById(Guid id);
    void Add(T entity);
    void Update(T entity);
    void Delete(Guid id);
}