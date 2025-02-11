using Application.DataTransferObjects.TagsDto;
using MediatR;

namespace Application.UseCases.Queries.TagQueries.GetTagByTaskId;

public record GetTagByTaskIdQuery(Guid TaskId) : IRequest<TagDto>
{
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
    public string TagName { get; set; }
}