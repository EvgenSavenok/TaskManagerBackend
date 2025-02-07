using Application.DataTransferObjects.TasksDto;
using MediatR;

namespace Application.UseCases.Queries.TaskQueries;

public record GetAllTasksQuery(bool TrackChanges) : IRequest<IEnumerable<TaskDto>>;