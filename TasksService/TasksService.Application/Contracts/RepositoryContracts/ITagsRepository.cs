using TasksService.Domain.Models;

namespace Application.Contracts.RepositoryContracts;

public interface ITagsRepository : IRepositoryBase<Tag>
{
    public Task<Tag?> GetTagByTaskId(
        Guid taskId, 
        Guid tagId,
        bool trackChanges, 
        CancellationToken cancellationToken);
    
    public Task<Tag?> GetTagByName(
        Guid taskId, 
        string tagName, 
        bool trackChanges, 
        CancellationToken cancellationToken);
}