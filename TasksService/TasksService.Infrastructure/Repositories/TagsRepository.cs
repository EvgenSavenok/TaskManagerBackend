using Application.Contracts.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class TagsRepository(ApplicationContext repositoryContext)
    : RepositoryBase<Tag>(repositoryContext), ITagsRepository
{
    public async Task<Tag?> GetTagById(
        Guid tagId,
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await repositoryContext.Tags
            .Where(tag => tag.Id == tagId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Tag?> GetTagByName(
        string tagName, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await repositoryContext.Tags
            .Where(tag => tag.Name == tagName)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public void Attach<T>(T entity) where T : class
    {
        repositoryContext.Attach(entity);
    }
}