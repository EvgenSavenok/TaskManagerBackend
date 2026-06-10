using Application.DataTransferObjects.CategoriesDto;
using MediatR;

namespace Application.UseCases.Queries.CategoryQueries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
