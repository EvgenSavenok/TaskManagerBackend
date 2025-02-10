using System.Linq.Expressions;
using TasksService.Domain.Models;

namespace Application.Contracts.RepositoryContracts;

public interface IRepositoryBase<T>
{
    Task<IEnumerable<T>> FindAll(bool trackChanges, CancellationToken cancellationToken);
    Task<IEnumerable<T>> FindByCondition(
        Expression<Func<T, bool>> expression,
        bool trackChanges,
        CancellationToken cancellationToken);
    Task Create(T entity, CancellationToken cancellationToken);
    Task Update(T entity, CancellationToken cancellationToken);
    Task Delete(T entity, CancellationToken cancellationToken);
}
