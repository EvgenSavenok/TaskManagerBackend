using Application.DataTransferObjects.TagsDto;
using MediatR;

namespace Application.UseCases.Queries.TagQueries.GetAllTagsOfTask;

public record GetAllTagsOfTaskQuery : IRequest<IEnumerable<TagDto>>
{
    public Guid TaskId { get; set; }
}