using MediatR;

namespace Application.UseCases.Commands.TaskCommands.DeleteTask;

public record DeleteTaskCommand : IRequest<Unit>
{
    public Guid TaskId { get; set; }
}