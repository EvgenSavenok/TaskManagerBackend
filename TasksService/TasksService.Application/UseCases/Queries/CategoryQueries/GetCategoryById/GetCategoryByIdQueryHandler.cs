using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CategoriesDto;
using AutoMapper;
using MediatR;
using TasksService.Domain.CustomExceptions;

namespace Application.UseCases.Queries.CategoryQueries.GetCategoryById;

public class GetCategoryByIdQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"category: {request.CategoryId}";

        var cached = await cache.GetAsync<CategoryDto>(cacheKey);
        if (cached != null)
            return cached;

        var category = await repository.Category.GetCategoryById(
            request.CategoryId,
            trackChanges: false,
            cancellationToken);

        if (category == null)
        {
            throw new NotFoundException($"Category with id {request.CategoryId} not found.");
        }

        var categoryDto = mapper.Map<CategoryDto>(category);

        await cache.SetAsync(cacheKey, categoryDto, TimeSpan.FromMinutes(10));

        return categoryDto;
    }
}
