using Application.Contracts.RepositoryContracts;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class TagsRepository(ApplicationContext repositoryContext)
    : RepositoryBase<Tag>(repositoryContext), ITagsRepository
{
    public async Task<Tag> GetTagByTaskId(
        Guid taskId, 
        Guid tagId,
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        IEnumerable<Tag?> tag = await FindByCondition(
            t => t.TaskTags.Any(task => task.Id == taskId) && (t.Id == tagId), 
            trackChanges, 
            cancellationToken);
        return tag.FirstOrDefault()!;
    }

    public async Task<Tag> GetTagByName(
        Guid taskId, 
        string tagName, 
        bool trackChanges, 
        CancellationToken cancellationToken)
    {
        IEnumerable<Tag?> tag = await FindByCondition(
            t => t.TaskTags.Any(task => task.Id == taskId) && (t.Name == tagName), 
            trackChanges, 
            cancellationToken);
        return tag.SingleOrDefault()!;
    }
}