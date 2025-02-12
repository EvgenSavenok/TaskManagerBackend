using MediatR;

namespace Application.UseCases.Commands.TagCommands.CreateTag;

public record CreateTagCommand : IRequest<Unit>
{
    public Guid TagId { get; set; }
    public string TagName { get; set; }
}