using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.TagQueries.GetAllTags;

public class GetAllTagsQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetAllTagsQuery, IEnumerable<TagDto>>
{
    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = "tags:all";
        
        var cachedTags = await cache.GetAsync<IEnumerable<TagDto>>(cacheKey);
        if (cachedTags != null) 
            return cachedTags;
        
        var tags = await repository.Tag.FindAll(trackChanges: false, cancellationToken: cancellationToken);

        IEnumerable<TagDto> tagsDto = mapper.Map<IEnumerable<TagDto>>(tags);
        
        await cache.SetAsync(cacheKey, tagsDto, TimeSpan.FromMinutes(10));
        
        return tagsDto;
    }
}