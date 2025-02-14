using MediatR;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;

public record CreateNotificationCommand : IRequest<Unit>
{
    public NotificationDto NotificationDto { get; set; }
}