using FluentValidation;
using TasksService.Domain.Models;

namespace Application.Validation;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(50).WithMessage("Category name must be at most 50 characters long.");
    }
}
