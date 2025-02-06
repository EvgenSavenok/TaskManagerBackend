using TasksService.Domain.Models;

namespace Application.Contracts.RepositoryContracts;

public interface ITasksRepository : IRepositoryBase<CustomTask>
{
    Task<IEnumerable<Task>> GetAllTasksAsync(bool trackChanges);
    Task<Task> GetTaskAsync(int bookId, bool trackChanges);
    Task<int> CountTasksAsync();
}