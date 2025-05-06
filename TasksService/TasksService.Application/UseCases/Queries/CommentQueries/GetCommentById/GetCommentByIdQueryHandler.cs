using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CommentsDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Queries.CommentQueries.GetCommentById;

public class GetCommentByIdQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetCommentByIdQuery, CommentDto>
{
    public async Task<CommentDto> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"comment: {request.CommentId}";

        var cachedComment = await cache.GetAsync<CommentDto>(cacheKey);
        if (cachedComment != null)
            return cachedComment;
        
        var comments = await repository.Comment.FindByCondition(
            c => c.Id == request.CommentId,
            trackChanges: false,
            cancellationToken);

        var comment = comments.FirstOrDefault();

        if (comment is null)
        {
            throw new NotFoundException($"Комментарий с ID {request.CommentId} не найден.");
        }

        var commentDto = mapper.Map<CommentDto>(comment);

        await cache.SetAsync(cacheKey, commentDto, TimeSpan.FromMinutes(10));

        return commentDto;
    }
}