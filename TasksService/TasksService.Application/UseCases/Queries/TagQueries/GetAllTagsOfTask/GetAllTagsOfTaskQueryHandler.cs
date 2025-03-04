using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.TagQueries.GetAllTagsOfTask;

public class GetAllTagsOfTaskQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetAllTagsOfTaskQuery, IEnumerable<TagDto>>
{
    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsOfTaskQuery request, CancellationToken cancellationToken)
    {
        var taskId = request.TaskId;
        
        var cacheKey = $"tags:{taskId}";
        
        var cachedTags = await cache.GetAsync<IEnumerable<TagDto>>(cacheKey);
        if (cachedTags != null)
            return cachedTags;

        var tags = await repository.Tag.FindByCondition(
            tag => tag.TaskTags.Any(task => task.Id == taskId), 
            trackChanges: false, 
            cancellationToken);

        var tagDtos = mapper.Map<IEnumerable<TagDto>>(tags);
        
        await cache.SetAsync(cacheKey, tagDtos, TimeSpan.FromMinutes(10));

        return tagDtos;
    }
}