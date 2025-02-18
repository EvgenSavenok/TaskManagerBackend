using MediatR;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;

namespace NotificationsService.Application.UseCases.Queries.NotificationQueries.GetAllNotifications;

public record GetAllNotificationsQuery : IRequest<IEnumerable<NotificationDto>>;