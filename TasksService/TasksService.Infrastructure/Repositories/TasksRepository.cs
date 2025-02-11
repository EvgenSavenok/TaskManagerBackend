using Application.Contracts.RepositoryContracts;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class TasksRepository(ApplicationContext repositoryContext)
    : RepositoryBase<CustomTask>(repositoryContext), ITasksRepository
{
    public async Task<CustomTask?> GetTaskByIdAsync(Guid taskId, bool trackChanges, CancellationToken cancellationToken)
    {
        IEnumerable<CustomTask?> task = await FindByCondition(c => c.Id.Equals(taskId), trackChanges, cancellationToken);
        return task.SingleOrDefault();
    }
}