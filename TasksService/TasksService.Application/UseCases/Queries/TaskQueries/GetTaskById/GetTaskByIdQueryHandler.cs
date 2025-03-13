using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Queries.TaskQueries.GetTaskById;

public class GetTaskByIdQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"task:{request.TaskId}";
        var cachedTask = await cache.GetAsync<TaskDto>(cacheKey);
        if (cachedTask != null)
        {
            return cachedTask;
        }

        Guid taskId = request.TaskId;
        
        var task = await repository.Task.GetTaskByIdAsync(taskId, trackChanges: false, cancellationToken);
        
        if (task == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found.");
        }
        
        var taskDto = mapper.Map<TaskDto>(task);
        
        await cache.SetAsync(cacheKey, taskDto, TimeSpan.FromMinutes(10));
        
        return taskDto;
    }
}