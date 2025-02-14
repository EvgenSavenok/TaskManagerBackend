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
    public virtual async Task<IEnumerable<T>> FindAll(bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges ?
            _repositoryContext.Set<T>()
                .AsNoTracking() :
            _repositoryContext.Set<T>()).ToListAsync(cancellationToken: cancellationToken);
    
    public async Task<IEnumerable<T>> FindByCondition(
        Expression<Func<T, bool>> expression,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _repositoryContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        query = query.Where(expression);

        if (includes is { Length: > 0 })
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        
        return await query.ToListAsync(cancellationToken);
    }

    
    public async Task Create(T entity, CancellationToken cancellationToken = default)
    {
        await _repositoryContext.Set<T>().AddAsync(entity, cancellationToken); 
        await _repositoryContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(T entity, CancellationToken cancellationToken = default)
    {
        _repositoryContext.Set<T>().Update(entity); 
        await _repositoryContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(T entity, CancellationToken cancellationToken = default)
    {
        _repositoryContext.Set<T>().Remove(entity);
        await _repositoryContext.SaveChangesAsync(cancellationToken);
    }
}