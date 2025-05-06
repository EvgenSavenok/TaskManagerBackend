using MediatR;

namespace Application.UseCases.Commands.CommentCommands.DeleteComment;

public record DeleteCommentCommand : IRequest<Unit>
{
    public Guid CommentId { get; set; }
}