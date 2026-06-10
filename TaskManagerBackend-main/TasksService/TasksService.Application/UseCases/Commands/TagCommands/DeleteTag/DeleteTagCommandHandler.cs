using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Commands.TagCommands.DeleteTag;

public class DeleteTagCommandHandler(
    IRepositoryManager repository,
    IRedisCacheService cache)
    : IRequestHandler<DeleteTagCommand>
{
    public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tagId = request.TagId;
        
        var tagEntity = await repository.Tag.GetTagById(
            tagId, 
            trackChanges: true, 
            cancellationToken);
        if (tagEntity == null)
        {
            throw new NotFoundException($"Tag with id {tagId} not found.");
        }

        await repository.Tag.Delete(tagEntity, cancellationToken);
        
        string cacheKey = "tags: all";
        await cache.RemoveAsync(cacheKey);
        
        return Unit.Value;
    }
}