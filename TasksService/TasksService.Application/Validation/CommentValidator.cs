using FluentValidation;
using TasksService.Domain.Models;

namespace Application.Validation;

public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(comment => comment.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(1000).WithMessage("Content must be at most 1000 characters long.");
    }
}