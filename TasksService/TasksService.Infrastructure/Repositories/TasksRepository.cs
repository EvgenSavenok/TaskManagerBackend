using Application.Contracts.RepositoryContracts;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class TasksRepository : RepositoryBase<CustomTask>, ITasksRepository
{
    public TasksRepository(ApplicationContext repositoryContext) : base(repositoryContext)
    {
    }

    public Task<IEnumerable<Task>> GetAllTasksAsync(bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public Task<Task> GetTaskAsync(int bookId, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountTasksAsync()
    {
        throw new NotImplementedException();
    }
}