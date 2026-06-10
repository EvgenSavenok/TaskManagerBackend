using Application.DataTransferObjects.CommentsDto;
using MediatR;

namespace Application.UseCases.Queries.CommentQueries.GetAllCommentsOfTask;

public record GetAllCommentsQuery : IRequest<IEnumerable<CommentDto>>
{
    public Guid TaskId { get; set; }
}