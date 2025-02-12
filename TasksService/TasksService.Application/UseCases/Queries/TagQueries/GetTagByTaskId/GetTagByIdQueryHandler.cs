using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Queries.TagQueries.GetTagByTaskId;

public class GetTagByIdQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetTagByIdQuery, TagDto>
{
    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        Guid tagId = request.TagId;
        var tag = await repository.Tag.GetTagById(tagId, trackChanges: false, cancellationToken);
        if (tag == null)
        {
            throw new NotFoundException($"Tag with id {tagId} not found.");
        }
        
        return mapper.Map<TagDto>(tag);
    }
}