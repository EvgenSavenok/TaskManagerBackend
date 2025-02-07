using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.TaskQueries;

public class GetAllTasksQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskDto>>
{
    public async Task<IEnumerable<TaskDto>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        var tasks = await repository.Task.FindAll(query.TrackChanges, cancellationToken);
        return mapper.Map<IEnumerable<TaskDto>>(tasks);
    }
}