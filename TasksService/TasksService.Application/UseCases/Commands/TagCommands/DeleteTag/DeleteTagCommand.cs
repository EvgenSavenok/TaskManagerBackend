using MediatR;

namespace Application.UseCases.Commands.TagCommands.DeleteTag;

public record DeleteTagCommand : IRequest<Unit>
{
    public Guid TagId { get; set; }
}