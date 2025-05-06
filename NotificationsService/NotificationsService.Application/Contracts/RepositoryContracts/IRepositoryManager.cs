using NotificationsService.Domain.Models;

namespace NotificationsService.Application.Contracts.RepositoryContracts;

public interface IRepositoryManager
{
    INotificationsRepository Notification { get; }
}