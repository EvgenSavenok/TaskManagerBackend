using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Queries.TaskQueries.GetTaskById;

public class GetTaskByIdQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        Guid taskId = request.TaskId;
        var task = await repository.Task.GetTaskByIdAsync(taskId, trackChanges: false, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException($"Task with id {taskId} not found.");
        }
        return mapper.Map<TaskDto>(task);
    }
}