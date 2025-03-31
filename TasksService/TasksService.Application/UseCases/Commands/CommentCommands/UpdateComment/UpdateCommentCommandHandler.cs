using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.CommentCommands.UpdateComment;

public class UpdateCommentCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache,
    IValidator<Comment> validator)
    : IRequestHandler<UpdateCommentCommand>
{
    public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
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
        
        mapper.Map(request, commentEntity);
        
        var validationResult = await validator.ValidateAsync(commentEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await repository.Comment.Update(commentEntity, cancellationToken);
        
        string cacheKey = $"comments: {commentEntity.TaskId}";
        await cache.RemoveAsync(cacheKey);
        
        return Unit.Value;
    }
}