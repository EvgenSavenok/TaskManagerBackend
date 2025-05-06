using MediatR;

namespace Application.UseCases.Commands.TagCommands.UpdateTag;

public record UpdateTagCommand : IRequest<Unit>
{
    public Guid TagId { get; set; }
    public string TagName { get; set; }
}