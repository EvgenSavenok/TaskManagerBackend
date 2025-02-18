using MediatR;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;

public record UpdateNotificationCommand : IRequest<Unit>
{
    public NotificationDto NotificationDto { get; init; }
}