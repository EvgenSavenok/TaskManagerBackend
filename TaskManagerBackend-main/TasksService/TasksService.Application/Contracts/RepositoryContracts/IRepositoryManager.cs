namespace Application.Contracts.RepositoryContracts;

public interface IRepositoryManager
{
    ITasksRepository Task { get; }
    ITagsRepository Tag { get; }
    ICommentsRepository Comment { get; }
}
