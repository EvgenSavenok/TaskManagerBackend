using MongoDB.Driver;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Domain.Models;

namespace NotificationsService.Infrastructure.Repositories;

public class NotificationsRepository(IMongoDatabase database)
    : RepositoryBase<Notification>(database, "Notifications"), INotificationsRepository;