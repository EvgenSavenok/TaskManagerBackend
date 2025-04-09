using Application.DataTransferObjects.TasksDto;
using MediatR;

namespace Application.UseCases.Commands.TaskCommands.CreateTask;

public record CreateTaskCommand : IRequest<CreateTaskResponseDto>
{
    public TaskDto TaskDto { get; set; } 
}
 