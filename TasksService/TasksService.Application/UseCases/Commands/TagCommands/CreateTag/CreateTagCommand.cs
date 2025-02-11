using MediatR;

namespace Application.UseCases.Commands.TagCommands.CreateTag;

public record CreateTagCommand : IRequest<Unit>
{
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
    public string TagName { get; set; }
}