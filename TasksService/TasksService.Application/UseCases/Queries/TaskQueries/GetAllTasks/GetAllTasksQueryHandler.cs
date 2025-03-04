using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using MediatR;
using Serilog;

namespace Application.UseCases.Queries.TaskQueries.GetAllTasks;

public class GetAllTasksQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskDto>>
{
    public async Task<IEnumerable<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        Log.Information("User message: {Message}", "Get all tasks");

        var userId = Guid.Parse(request.UserId.Value);
        string cacheKey = $"tasks:user:{userId}";
          
        var cachedTasks = await cache.GetAsync<IEnumerable<TaskDto>>(cacheKey);
        if (cachedTasks != null) 
            return cachedTasks;
        
        var tasks = await repository.Task.FindByCondition(
            task => task.UserId == userId, 
            trackChanges: false,
            cancellationToken,
            t => t.TaskTags,
            t => t.TaskComments);
        
        var taskDtos = mapper.Map<IEnumerable<TaskDto>>(tasks);
        
        await cache.SetAsync(cacheKey, taskDtos, TimeSpan.FromMinutes(10));
        
        return taskDtos;
    }
}