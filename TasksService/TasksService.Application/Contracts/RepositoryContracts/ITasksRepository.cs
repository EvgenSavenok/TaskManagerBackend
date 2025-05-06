using System.Collections;
using TasksService.Domain.Models;

namespace Application.Contracts.RepositoryContracts;

public interface ITasksRepository : IRepositoryBase<CustomTask>
{
    Task<CustomTask?> GetTaskByIdAsync(
        Guid taskId,
        bool trackChanges, 
        CancellationToken cancellationToken);

    Task<IEnumerable<CustomTask>> GetAllTasks(
        bool trackChanges, 
        CancellationToken cancellationToken,
        Guid userId);
}