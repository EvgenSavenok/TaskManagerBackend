using Application.Contracts.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class TagsRepository(ApplicationContext repositoryContext)
    : RepositoryBase<Tag>(repositoryContext), ITagsRepository
{
    public async Task<Tag?> GetTagByTaskId(
        Guid taskId, 
        Guid tagId,
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await repositoryContext.Tasks
            .Where(task => task.Id == taskId)
            .SelectMany(task => task.TaskTags)
            .Where(tag => tag.Id == tagId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Tag?> GetTagByName(
        Guid taskId, 
        string tagName, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        return await repositoryContext.Tasks
            .Where(task => task.Id == taskId)
            .SelectMany(task => task.TaskTags)
            .Where(tag => tag.Name == tagName)
            .FirstOrDefaultAsync(cancellationToken);
    }
}