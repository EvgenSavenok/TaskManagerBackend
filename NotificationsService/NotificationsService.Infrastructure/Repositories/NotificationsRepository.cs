using MongoDB.Driver;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Domain.Models;

namespace NotificationsService.Infrastructure.Repositories;

public class NotificationsRepository(IMongoDatabase database)
    : RepositoryBase<Notification>(database, "Notifications"), INotificationsRepository
{
    public async Task UpdateNotificationByTaskId(Notification notification, CancellationToken cancellationToken)
    {
        var filter = Builders<Notification>.Filter.Eq(n => n.TaskId, notification.TaskId);

        var update = Builders<Notification>.Update
            .Set(n => n.Title, notification.Title)
            .Set(n => n.CreatedAt, notification.CreatedAt)
            .Set(n => n.Deadline, notification.Deadline)
            .Set(n => n.MinutesBeforeDeadline, notification.MinutesBeforeDeadline)
            .Set(n => n.UserEmail, notification.UserEmail)
            .Set(n => n.UserTimeZone, notification.UserTimeZone)
            .Set(n => n.HangfireJobId, notification.HangfireJobId);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}