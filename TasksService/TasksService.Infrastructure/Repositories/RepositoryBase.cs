using System.Linq.Expressions;
using Application.Contracts.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace TasksService.Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly ApplicationContext _repositoryContext;

    protected RepositoryBase(ApplicationContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }
    public async Task<IEnumerable<T>> FindAll(bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges ?
            _repositoryContext.Set<T>()
                .AsNoTracking() :
            _repositoryContext.Set<T>()).ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges ?
            _repositoryContext.Set<T>()
                .Where(expression)
                .AsNoTracking() :
            _repositoryContext.Set<T>()
                .Where(expression)).ToListAsync(cancellationToken);
    
    public void Create(T entity) => _repositoryContext.Set<T>().Add(entity);
    public void Update(T entity) => _repositoryContext.Set<T>().Update(entity);
    public void Delete(T entity) => _repositoryContext.Set<T>().Remove(entity);
}