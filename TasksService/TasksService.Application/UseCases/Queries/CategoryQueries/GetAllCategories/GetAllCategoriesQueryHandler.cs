using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CategoriesDto;
using AutoMapper;
using MediatR;

namespace Application.UseCases.Queries.CategoryQueries.GetAllCategories;

public class GetAllCategoriesQueryHandler(
    IRepositoryManager repository,
    IMapper mapper,
    IRedisCacheService cache)
    : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "categories: all";

        var cached = await cache.GetAsync<IEnumerable<CategoryDto>>(cacheKey);
        if (cached != null)
            return cached;

        var categories = await repository.Category.FindAll(trackChanges: false, cancellationToken: cancellationToken);

        var categoriesDto = mapper.Map<IEnumerable<CategoryDto>>(categories);

        await cache.SetAsync(cacheKey, categoriesDto, TimeSpan.FromMinutes(10));

        return categoriesDto;
    }
}
