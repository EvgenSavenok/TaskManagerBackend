using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Validation;
using NotificationsService.Infrastructure.Repositories;

namespace NotificationsService.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var mongoSettings = configuration.GetConnectionString("MongoDb");
        var client = new MongoClient(mongoSettings);
        var database = client.GetDatabase("NotificationsDb");

        services.AddSingleton(database);
        services.AddScoped<INotificationsRepository, NotificationsRepository>();

        return services;
    }
    
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<NotificationValidator>();
    }
}