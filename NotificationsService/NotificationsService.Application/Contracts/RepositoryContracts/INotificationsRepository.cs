using NotificationsService.Domain.Models;

namespace NotificationsService.Application.Contracts.RepositoryContracts;

public interface INotificationsRepository : IRepositoryBase<Notification>
{
    public Task UpdateNotificationByTaskId(Notification notification, CancellationToken cancellationToken);
}