using Application.DataTransferObjects.TasksDto;
using MediatR;

namespace Application.UseCases.Commands.TaskCommands.UpdateTask;

public record UpdateTaskCommand : IRequest<Unit>
{
    public TaskDto TaskDto { get; set; } 
}