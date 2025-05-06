using FluentValidation;
using TasksService.Domain.Models;

namespace Application.Validation;

public class TagValidator : AbstractValidator<Tag>
{
    public TagValidator()
    {
        RuleFor(tag => tag.Name)
            .NotEmpty().WithMessage("Tag name is required.")
            .MaximumLength(50).WithMessage("Tag name must be at most 50 characters long.");
    }
}