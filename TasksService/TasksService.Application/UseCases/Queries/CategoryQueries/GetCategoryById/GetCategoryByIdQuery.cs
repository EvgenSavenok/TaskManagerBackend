using Application.DataTransferObjects.CategoriesDto;
using MediatR;

namespace Application.UseCases.Queries.CategoryQueries.GetCategoryById;

public record GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid CategoryId { get; set; }
}
