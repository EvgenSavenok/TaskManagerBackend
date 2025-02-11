using MediatR;

namespace Application.UseCases.Commands.TagCommands.UpdateTag;

public record UpdateTagCommand : IRequest<Unit>
{
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
    public string TagName { get; set; }
}