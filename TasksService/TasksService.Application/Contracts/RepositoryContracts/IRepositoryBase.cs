using System.Linq.Expressions;
using TasksService.Domain.Models;

namespace Application.Contracts.RepositoryContracts;

public interface IRepositoryBase<T>
{
    public Task<IEnumerable<T>> FindAll(bool trackChanges, CancellationToken cancellationToken);
    public Task<IEnumerable<T>> FindByCondition(
        Expression<Func<T, bool>> expression,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);
    Task Create(T entity, CancellationToken cancellationToken);
    Task Update(T entity, CancellationToken cancellationToken);
    Task Delete(T entity, CancellationToken cancellationToken);
}
