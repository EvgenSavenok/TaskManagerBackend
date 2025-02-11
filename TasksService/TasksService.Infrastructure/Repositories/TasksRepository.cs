using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Repositories;

public class TasksRepository(ApplicationContext repositoryContext)
    : RepositoryBase<CustomTask>(repositoryContext), ITasksRepository
{
    public async Task<CustomTask?> GetTaskByIdAsync(Guid taskId, bool trackChanges, CancellationToken cancellationToken)
    {
        var taskWithTags = await FindByCondition(
            t => t.Id == taskId, 
            trackChanges: false, 
            cancellationToken, 
            t => t.TaskTags);
        return taskWithTags.FirstOrDefault();
    }
}