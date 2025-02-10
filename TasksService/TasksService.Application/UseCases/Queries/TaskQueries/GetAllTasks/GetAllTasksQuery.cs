using Application.DataTransferObjects.TasksDto;
using MediatR;

namespace Application.UseCases.Queries.TaskQueries.GetAllTasks;

public record GetAllTasksQuery() : IRequest<IEnumerable<TaskDto>>;