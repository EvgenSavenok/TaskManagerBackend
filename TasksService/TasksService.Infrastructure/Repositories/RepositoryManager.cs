using Application.Contracts.RepositoryContracts;

namespace TasksService.Infrastructure.Repositories;

public class RepositoryManager(ApplicationContext repositoryContext) : IRepositoryManager
{
    private ITasksRepository _taskRepository;
    private ITagsRepository _tagRepository;
    private ICommentsRepository _commentRepository;

    public ITasksRepository Task
    {
        get
        {
            if(_taskRepository == null)
                _taskRepository = new TasksRepository(repositoryContext);
            return _taskRepository;
        }
    }
    
    public ITagsRepository Tag
    {
        get
        {
            if(_tagRepository == null)
                _tagRepository = new TagsRepository(repositoryContext);
            return _tagRepository;
        }
    }
    
    public ICommentsRepository Comment
    {
        get
        {
            if(_commentRepository == null)
                _commentRepository = new CommentsRepository(repositoryContext);
            return _commentRepository;
        }
    }
}
