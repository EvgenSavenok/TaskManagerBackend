using MediatR;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;

namespace NotificationsService.Application.UseCases.Queries.NotificationQueries.GetNotificationById;

public record GetNotificationQuery : IRequest<NotificationDto>
{
   public Guid NotificationId { get; init; } 
}