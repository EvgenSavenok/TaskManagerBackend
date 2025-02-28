using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.TaskQueries.GetAllTasks;

public class GetAllTasksQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskDto>>
{
    public async Task<IEnumerable<TaskDto>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(query.UserId.Value);
        
        var tasks = await repository.Task.FindByCondition(
            task => task.UserId == userId, 
            trackChanges: false,
            cancellationToken,
            t => t.TaskTags,
            t => t.TaskComments);
        
        return mapper.Map<IEnumerable<TaskDto>>(tasks);
    }
}