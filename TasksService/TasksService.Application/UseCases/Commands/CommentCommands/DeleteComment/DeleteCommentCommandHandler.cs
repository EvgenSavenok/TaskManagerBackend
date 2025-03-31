using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Commands.CommentCommands.DeleteComment;

public class DeleteCommentCommandHandler(
    IRepositoryManager repository,
    IRedisCacheService cache)
    : IRequestHandler<DeleteCommentCommand>
{
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var commentId = request.CommentId;
        
        var comments = await repository.Comment.FindByCondition(
            c => c.Id == request.CommentId,
            trackChanges: false,
            cancellationToken);
        var commentEntity = comments.FirstOrDefault();
        if (commentEntity == null)
        {
            throw new NotFoundException($"Comment with id {commentId} not found");
        }

        await repository.Comment.Delete(commentEntity, cancellationToken);
        
        string cacheKey = $"comments: {commentEntity.TaskId}";
        await cache.RemoveAsync(cacheKey);
        
        return Unit.Value;
    }
}