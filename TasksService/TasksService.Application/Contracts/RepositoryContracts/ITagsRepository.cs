using TasksService.Domain.Models;

namespace Application.Contracts.RepositoryContracts;

public interface ITagsRepository : IRepositoryBase<Tag>
{
    public Task<Tag?> GetTagById(
        Guid tagId,
        bool trackChanges, 
        CancellationToken cancellationToken);
    
    public Task<Tag?> GetTagByName(
        string tagName, 
        bool trackChanges, 
        CancellationToken cancellationToken);
    
    void Attach<T>(T entity) where T : class;
}