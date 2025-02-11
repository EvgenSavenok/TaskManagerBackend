using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Queries.TagQueries.GetTagByTaskId;

public class GetTagByTaskIdQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetTagByTaskIdQuery, TagDto>
{
    public async Task<TagDto> Handle(GetTagByTaskIdQuery request, CancellationToken cancellationToken)
    {
        Guid taskId = request.TaskId;
        Guid tagId = request.TagId;
        var tag = await repository.Tag.GetTagByTaskId(taskId, tagId, trackChanges: false, cancellationToken);
        if (tag == null)
        {
            throw new NotFoundException($"Tag with id {tagId} not found.");
        }
        
        return mapper.Map<TagDto>(tag);
    }
}