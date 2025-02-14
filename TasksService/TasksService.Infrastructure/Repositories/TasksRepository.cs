using Application.Contracts.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class TasksRepository(ApplicationContext repositoryContext)
    : RepositoryBase<CustomTask>(repositoryContext), ITasksRepository
{
    public async Task<CustomTask?> GetTaskByIdAsync(Guid taskId, bool trackChanges, CancellationToken cancellationToken)
    {
        var tasks = await FindByCondition(
            t => t.Id == taskId, 
            trackChanges, 
            cancellationToken, 
            t => t.TaskTags,
            t => t.TaskComments);
        return tasks.FirstOrDefault();
    }
    
    public override async Task<IEnumerable<CustomTask>> FindAll(bool trackChanges, CancellationToken cancellationToken)
    {
        var query = repositoryContext.Tasks.AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        query = query
            .Include(t => t.TaskTags)    
            .Include(t => t.TaskComments); 

        return await query.ToListAsync(cancellationToken);
    }
}