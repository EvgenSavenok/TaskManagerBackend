using Application.UseCases.Commands.TaskCommands.CreateTask;
using FluentValidation;
using TasksService.Domain.Models;

namespace Application.Validation;

public class TaskValidator : AbstractValidator<CustomTask>
{
    public TaskValidator()
    {
        RuleFor(task => task.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 100 characters");
        
        RuleFor(task => task.CreatedAt)
            .NotEmpty().WithMessage("CreatedAt is required.")
            .Must(createdAt => createdAt <= DateTime.UtcNow)
            .WithMessage("CreatedAt cannot be in the future.");

        RuleFor(task => task.Deadline)
            .NotEmpty().WithMessage("Deadline is required.")
            .Must((notification, deadline) => deadline > notification.CreatedAt)
            .WithMessage("Deadline must be after CreatedAt.")
            .Must((task, deadline) => (deadline - task.CreatedAt).TotalMinutes >= 1)
            .WithMessage("The difference between CreatedAt and Deadline must be at least one minute.");
        
        RuleFor(task => task.Category)
            .NotEmpty().WithMessage("Category is required.");
        
        RuleFor(task => task.Priority)
            .NotEmpty().WithMessage("Priority is required.");
        
        RuleFor(task => task.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
    }
}