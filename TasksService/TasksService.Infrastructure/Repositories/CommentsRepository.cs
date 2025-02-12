using Application.Contracts.RepositoryContracts;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class CommentsRepository(ApplicationContext repositoryContext)
    : RepositoryBase<Comment>(repositoryContext), ICommentsRepository
{
    
}