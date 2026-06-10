using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using AutoMapper;
using FluentValidation;
using MediatR;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.CategoryCommands.CreateCategory;

public class CreateCategoryCommandHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache,
    IValidator<Category> validator)
    : IRequestHandler<CreateCategoryCommand, Category>
{
    public async Task<Category> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryEntity = mapper.Map<Category>(request);

        var validationResult = await validator.ValidateAsync(categoryEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existing = await repository.Category.GetCategoryByName(
            categoryEntity.Name,
            trackChanges: false,
            cancellationToken);

        if (existing != null)
        {
            throw new AlreadyExistsException($"Category with name '{categoryEntity.Name}' already exists.");
        }

        await repository.Category.Create(categoryEntity, cancellationToken);

        await cache.RemoveAsync("categories: all");

        return categoryEntity;
    }
}
