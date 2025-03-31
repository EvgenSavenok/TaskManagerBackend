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
    public async Task<IEnumerable<TaskDto>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(query.UserId.Value);
        
        var tasks = await repository.Task.GetAllTasks(
            trackChanges: false,
            cancellationToken,
            userId);
        
        var taskDtos = mapper.Map<IEnumerable<TaskDto>>(tasks);
        
        return taskDtos;
    }
}