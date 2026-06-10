using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.CategoryCommands.UpdateCategory;

public class UpdateCategoryCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache,
    IValidator<Category> validator)
    : IRequestHandler<UpdateCategoryCommand>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryEntity = await repository.Category.GetCategoryById(
            request.CategoryId,
            trackChanges: true,
            cancellationToken);

        if (categoryEntity == null)
        {
            throw new NotFoundException($"Category with id {request.CategoryId} not found.");
        }

        mapper.Map(request, categoryEntity);

        var validationResult = await validator.ValidateAsync(categoryEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await repository.Category.Update(categoryEntity, cancellationToken);

        await cache.RemoveAsync("categories: all");
        await cache.RemoveAsync($"category: {request.CategoryId}");

        return Unit.Value;
    }
}
