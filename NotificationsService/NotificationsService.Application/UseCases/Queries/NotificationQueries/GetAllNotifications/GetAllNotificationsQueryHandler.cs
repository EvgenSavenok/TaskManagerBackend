using AutoMapper;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;

namespace NotificationsService.Application.UseCases.Queries.NotificationQueries.GetAllNotifications;

public class GetAllNotificationsQueryHandler(
    IRepositoryManager repository,
    IMapper mapper)
    : IRequestHandler<GetAllNotificationsQuery, IEnumerable<NotificationDto>>
{
    public async Task<IEnumerable<NotificationDto>> Handle(
        GetAllNotificationsQuery request, 
        CancellationToken cancellationToken)
    {
        var notifications = await repository.Notification.FindAll(cancellationToken); 

        return mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}