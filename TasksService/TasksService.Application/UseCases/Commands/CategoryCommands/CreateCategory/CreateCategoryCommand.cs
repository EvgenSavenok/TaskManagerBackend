using MediatR;
using TasksService.Domain.Models;

namespace Application.UseCases.Commands.CategoryCommands.CreateCategory;

public record CreateCategoryCommand : IRequest<Category>
{
    public string CategoryName { get; set; }
}
