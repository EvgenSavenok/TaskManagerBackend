using Application.Contracts.RepositoryContracts;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Commands.TagCommands.DeleteTag;

public class DeleteTagCommandHandler(IRepositoryManager repository)
    : IRequestHandler<DeleteTagCommand>
{
    public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tagId = request.TagId;
        var taskId = request.TaskId;
        
        var tagEntity = await repository.Tag.GetTagByTaskId(
            taskId, 
            tagId, 
            trackChanges: true, 
            cancellationToken);
        if (tagEntity == null)
        {
            throw new NotFoundException($"Tag with id {tagId} not found.");
        }

        await repository.Tag.Delete(tagEntity, cancellationToken);
        
        return Unit.Value;
    }
}