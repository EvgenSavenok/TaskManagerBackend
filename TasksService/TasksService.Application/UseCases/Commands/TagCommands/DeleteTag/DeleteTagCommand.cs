using MediatR;

namespace Application.UseCases.Commands.TagCommands.DeleteTag;

public record DeleteTagCommand : IRequest<Unit>
{
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
}