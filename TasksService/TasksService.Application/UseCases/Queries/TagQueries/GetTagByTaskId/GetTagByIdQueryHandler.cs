using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Queries.TagQueries.GetTagByTaskId;

public class GetTagByIdQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetTagByIdQuery, TagDto>
{
    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        Guid tagId = request.TagId;
        string cacheKey = $"tag: {tagId}";
        
        var cachedTag = await cache.GetAsync<TagDto>(cacheKey);
        if (cachedTag != null) 
            return cachedTag;
        
        var tag = await repository.Tag.GetTagById(tagId, trackChanges: false, cancellationToken);
        if (tag == null)
        {
            throw new NotFoundException($"Tag with id {tagId} not found.");
        }
        
        var tagDto = mapper.Map<TagDto>(tag);
        
        await cache.SetAsync(cacheKey, tagDto, TimeSpan.FromMinutes(10));

        return tagDto;
    }
}