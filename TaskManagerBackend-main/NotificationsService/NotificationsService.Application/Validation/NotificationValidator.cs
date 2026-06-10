using FluentValidation;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.Validation;

public class NotificationValidator : AbstractValidator<Notification>
{
    public NotificationValidator()
    {
        RuleFor(notification => notification.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must be at most 100 characters long.");
        
        RuleFor(notification => notification.MinutesBeforeDeadline)
            .NotEmpty().WithMessage("MinutesBeforeDeadline is required.")
            .GreaterThanOrEqualTo(0).WithMessage("MinutesBeforeDeadline must be non-negative.");
        
        RuleFor(notification => notification.CreatedAt)
            .NotEmpty().WithMessage("CreatedAt is required.")
            .Must(createdAt => createdAt <= DateTime.UtcNow)
            .WithMessage("CreatedAt cannot be in the future.");
        
        RuleFor(notification => notification.Deadline)
            .NotEmpty().WithMessage("Deadline is required.")
            .Must((notification, deadline) => deadline > notification.CreatedAt)
            .WithMessage("Deadline must be after CreatedAt.");
        
        RuleFor(notification => notification.ReminderTime)
            .NotEmpty().WithMessage("ReminderTime is required.")
            .Must((notification, reminderTime) => reminderTime <= notification.Deadline 
                                                  && reminderTime >= notification.CreatedAt)
            .WithMessage("ReminderTime must be between CreatedAt and Deadline.");
    }
}