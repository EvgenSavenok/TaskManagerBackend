using Application.DataTransferObjects.TasksDto;
using MediatR;

namespace Application.UseCases.Queries.TaskQueries.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskDto>;