using Application.DataTransferObjects.CommentsDto;
using MediatR;

namespace Application.UseCases.Queries.CommentQueries.GetCommentById;

public class GetCommentByIdQuery : IRequest<CommentDto>
{
    public Guid CommentId { get; set; }
}