using System.Linq.Expressions;
using Application.Contracts.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected ApplicationContext RepositoryContext;
    public RepositoryBase(ApplicationContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
    }
    public async Task<IEnumerable<T>> FindAll(bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges ?
            RepositoryContext.Set<T>()
                .AsNoTracking() :
            RepositoryContext.Set<T>()).ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges ?
            RepositoryContext.Set<T>()
                .Where(expression)
                .AsNoTracking() :
            RepositoryContext.Set<T>()
                .Where(expression)).ToListAsync(cancellationToken);
    
    public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
    public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
    public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
}