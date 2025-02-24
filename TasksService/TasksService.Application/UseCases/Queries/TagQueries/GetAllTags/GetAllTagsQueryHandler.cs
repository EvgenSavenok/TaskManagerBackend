using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.Models;

namespace Application.UseCases.Queries.TagQueries.GetAllTags;

public class GetAllTagsQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetAllTagsQuery, IEnumerable<TagDto>>
{
    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await repository.Tag.FindAll(trackChanges: false, cancellationToken: cancellationToken);

        IEnumerable<TagDto> tagsDto = mapper.Map<IEnumerable<TagDto>>(tags);
        
        return tagsDto;
    }
}