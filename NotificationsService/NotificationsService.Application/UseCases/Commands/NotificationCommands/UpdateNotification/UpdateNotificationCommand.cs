using MediatR;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Domain.Enums;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;

public record UpdateNotificationCommand : IRequest<Unit>
{
    public NotificationDto NotificationDto { get; init; }
    
    public Status Status { get; init; }
}