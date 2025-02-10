namespace Application.Contracts.RepositoryContracts;

public interface IRepositoryManager
{
    ITasksRepository Task { get; }
}
