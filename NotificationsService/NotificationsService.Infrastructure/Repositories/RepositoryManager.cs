using MongoDB.Driver;
using NotificationsService.Application.Contracts.RepositoryContracts;

namespace NotificationsService.Infrastructure.Repositories;

public class RepositoryManager(IMongoDatabase database) : IRepositoryManager
{
    private INotificationsRepository? _notificationRepository;

    public INotificationsRepository Notification =>
        _notificationRepository ??= new NotificationsRepository(database);
}