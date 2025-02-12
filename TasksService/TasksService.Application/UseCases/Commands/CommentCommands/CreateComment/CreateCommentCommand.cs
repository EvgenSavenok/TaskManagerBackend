using MediatR;

namespace Application.UseCases.Commands.CommentCommands.CreateComment;

public record CreateCommentCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Content { get; set; }
}