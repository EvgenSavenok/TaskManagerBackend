using MediatR;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.DeleteNotification;

public record DeleteNotificationCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}