namespace Application.Contracts.RepositoryContracts;

public interface IRepositoryManager
{
    ITasksRepository Task { get; }
    Task SaveAsync();
}
