using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CommentsDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.CommentQueries.GetAllCommentsOfTask;

public class GetAllCommentsOfTaskQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetAllCommentsQuery, IEnumerable<CommentDto>>
{
    public async Task<IEnumerable<CommentDto>> Handle(
        GetAllCommentsQuery request, 
        CancellationToken cancellationToken)
    {
        var taskId = request.TaskId;

        var comments = await repository.Comment.FindByCondition(
            comment => comment.TaskId == taskId,  
            trackChanges: false, 
            cancellationToken); 

        return mapper.Map<IEnumerable<CommentDto>>(comments);
    }

}