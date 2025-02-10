using Application.Contracts.RepositoryContracts;

namespace TasksService.Infrastructure.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private ApplicationContext _repositoryContext;
    private ITasksRepository _taskRepository;

    public RepositoryManager(ApplicationContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }
    
    public ITasksRepository Task
    {
        get
        {
            if(_taskRepository == null)
                _taskRepository = new TasksRepository(_repositoryContext);
            return _taskRepository;
        }
    }
    
    public Task SaveAsync() => _repositoryContext.SaveChangesAsync();
}
