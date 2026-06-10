using MediatR;

namespace Application.UseCases.Commands.CategoryCommands.UpdateCategory;

public record UpdateCategoryCommand : IRequest<Unit>
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
}
