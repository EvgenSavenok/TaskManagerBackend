using AutoMapper;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Domain.CustomExceptions;

namespace NotificationsService.Application.UseCases.Queries.NotificationQueries.GetNotificationById;

public class GetNotificationQueryHandler( 
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetNotificationQuery, NotificationDto>
{
    public async Task<NotificationDto> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        var notifications = await repository.Notification.FindByCondition(
            notification => notification.Id == request.NotificationId,
            cancellationToken);

        var notification = notifications.FirstOrDefault();

        if (notification is null)
        {
            throw new NotFoundException($"Уведомление с ID {request.NotificationId} не найдено.");
        }

        return mapper.Map<NotificationDto>(notification);
    }
}