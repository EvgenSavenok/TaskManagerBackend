using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CommentsDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.CommentQueries.GetAllCommentsOfTask;

public class GetAllCommentsOfTaskQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetAllCommentsQuery, IEnumerable<CommentDto>>
{
    public async Task<IEnumerable<CommentDto>> Handle(
        GetAllCommentsQuery request, 
        CancellationToken cancellationToken)
    {
        var taskId = request.TaskId;
        
        var cacheKey = $"comments: {taskId}";

        var cachedComments = await cache.GetAsync<IEnumerable<CommentDto>>(cacheKey);
        if (cachedComments != null)
            return cachedComments;

        var comments = await repository.Comment.FindByCondition(
            comment => comment.TaskId == taskId,  
            trackChanges: false, 
            cancellationToken);

        var commentDtos = mapper.Map<IEnumerable<CommentDto>>(comments);
        
        await cache.SetAsync(cacheKey, commentDtos, TimeSpan.FromMinutes(10));
        
        return commentDtos;
    }
}