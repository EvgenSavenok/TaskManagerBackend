using Application.DataTransferObjects.TagsDto;
using MediatR;

namespace Application.UseCases.Queries.TagQueries.GetAllTags;

public record GetAllTagsQuery : IRequest<IEnumerable<TagDto>>
{
    public Guid TaskId { get; set; }
}