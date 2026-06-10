using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Commands.CategoryCommands.DeleteCategory;

public class DeleteCategoryCommandHandler(
    IRepositoryManager repository,
    IRedisCacheService cache)
    : IRequestHandler<DeleteCategoryCommand>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryEntity = await repository.Category.GetCategoryById(
            request.CategoryId,
            trackChanges: true,
            cancellationToken);

        if (categoryEntity == null)
        {
            throw new NotFoundException($"Category with id {request.CategoryId} not found.");
        }

        await repository.Category.Delete(categoryEntity, cancellationToken);

        await cache.RemoveAsync("categories: all");
        await cache.RemoveAsync($"category: {request.CategoryId}");

        return Unit.Value;
    }
}
