using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.TagQueries.GetAllTagsOfTask;

public class GetAllTagsOfTaskQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetAllTagsOfTaskQuery, IEnumerable<TagDto>>
{
    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsOfTaskQuery request, CancellationToken cancellationToken)
    {
        var taskId = request.TaskId;

        var tags = await repository.Tag.FindByCondition(
            tag => tag.TaskTags.Any(task => task.Id == taskId), 
            trackChanges: false, 
            cancellationToken);

        return mapper.Map<IEnumerable<TagDto>>(tags);
    }
}