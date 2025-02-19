using MediatR;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;

public record CreateNotificationCommand : IRequest<Notification>
{
    public NotificationDto NotificationDto { get; set; }
}