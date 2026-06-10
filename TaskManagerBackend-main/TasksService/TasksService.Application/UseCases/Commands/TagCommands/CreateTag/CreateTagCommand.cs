using MediatR;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.TagCommands.CreateTag;

public record CreateTagCommand : IRequest<Tag>
{
    public Guid TagId { get; set; }
    public string TagName { get; set; }
}