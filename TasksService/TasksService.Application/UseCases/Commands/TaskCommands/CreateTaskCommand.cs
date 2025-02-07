using MediatR;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TaskCommands;

public record CreateTaskCommand : IRequest<Unit>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public Priority Priority { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
}
 