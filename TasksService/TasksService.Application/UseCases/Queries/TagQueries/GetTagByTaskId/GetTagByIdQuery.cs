using Application.DataTransferObjects.TagsDto;
using MediatR;

namespace Application.UseCases.Queries.TagQueries.GetTagByTaskId;

public record GetTagByIdQuery : IRequest<TagDto>
{
    public Guid TagId { get; set; }
    public string TagName { get; set; }
}