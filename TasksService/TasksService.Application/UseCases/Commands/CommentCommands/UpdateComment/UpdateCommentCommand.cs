using MediatR;

namespace Application.UseCases.Commands.CommentCommands.UpdateComment;

public record UpdateCommentCommand : IRequest<Unit>
{
    public Guid CommentId { get; set; }
    public string Content { get; set; }
}