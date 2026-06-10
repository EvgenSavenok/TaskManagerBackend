using MediatR;

namespace Application.UseCases.Commands.CategoryCommands.DeleteCategory;

public record DeleteCategoryCommand : IRequest<Unit>
{
    public Guid CategoryId { get; set; }
}
