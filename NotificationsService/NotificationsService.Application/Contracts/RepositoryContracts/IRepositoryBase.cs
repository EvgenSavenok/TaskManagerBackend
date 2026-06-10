using System.Linq.Expressions;

namespace NotificationsService.Application.Contracts.RepositoryContracts;

public interface IRepositoryBase<T>
{
    public Task<IEnumerable<T>> FindAll(CancellationToken cancellationToken);
    public Task<IEnumerable<T>> FindByCondition(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken);
    Task Create(T entity, CancellationToken cancellationToken);
    Task Update(T entity, CancellationToken cancellationToken);
    Task Delete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
}
