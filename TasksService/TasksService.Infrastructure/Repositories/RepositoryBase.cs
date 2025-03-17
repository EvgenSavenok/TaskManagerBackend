using System.Linq.Expressions;
using Application.Contracts.RepositoryContracts;
using Application.Contracts.Specification;
using Microsoft.EntityFrameworkCore;

namespace TasksService.Infrastructure.Repositories;

public abstract class RepositoryBase<T>(ApplicationContext repositoryContext) : IRepositoryBase<T>
    where T : class
{
    public virtual async Task<IEnumerable<T>> FindAll(bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges ?
            repositoryContext.Set<T>()
                .AsNoTracking() :
            repositoryContext.Set<T>()).ToListAsync(cancellationToken: cancellationToken);
    
    public async Task<IEnumerable<T>> FindByCondition(
        Expression<Func<T, bool>> expression,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = repositoryContext.Set<T>();

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
        await repositoryContext.Set<T>().AddAsync(entity, cancellationToken); 
        await repositoryContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(T entity, CancellationToken cancellationToken = default)
    {
        repositoryContext.Set<T>().Update(entity); 
        await repositoryContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(T entity, CancellationToken cancellationToken = default)
    {
        repositoryContext.Set<T>().Remove(entity);
        await repositoryContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<T>> GetBySpecificationAsync(ISpecification<T> specification, bool trackChanges,
        CancellationToken cancellationToken)
    {
        IQueryable<T> query = repositoryContext.Set<T>();

        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);
        
        if (!trackChanges)
            query = query.AsNoTracking();

        if (specification.OrderBy != null)
            query = specification.OrderBy(query);

        if (specification.Includes != null)
            query = specification.Includes(query);

        query = query
            .Skip((specification.PageNumber - 1) * specification.PageSize)
            .Take(specification.PageSize);

        return await query.ToListAsync(cancellationToken);
    }
}