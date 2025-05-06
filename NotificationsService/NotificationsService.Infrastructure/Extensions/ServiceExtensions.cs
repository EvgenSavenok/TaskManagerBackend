using System.Text;
using FluentValidation;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Application.EmailService;
using NotificationsService.Application.Validation;
using NotificationsService.Infrastructure.Messaging;
using NotificationsService.Infrastructure.Repositories;
using RabbitMQ.Client;
using Serilog;

namespace NotificationsService.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void AddMongoDb(this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoSettings = configuration.GetConnectionString("MongoDb");
        var client = new MongoClient(mongoSettings);
        var database = client.GetDatabase("NotificationsDb");

        services.AddSingleton(database);
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
    }
    
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<NotificationValidator>();
    }

    public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config.UseMongoStorage(
            configuration.GetConnectionString("MongoHangfire"), 
            new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                    { MigrationStrategy = new DropMongoMigrationStrategy() },
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
            }
        ));
        //services.AddHangfireServer();
    }
    
    public static void UpdateJobGraphState(this IServiceProvider serviceProvider)
    {
        var client = new MongoClient("mongodb://mongo:27017");
        var database = client.GetDatabase("HangfireDb");
        var collection = database.GetCollection<BsonDocument>("hangfire.jobGraph");

        var filter = Builders<BsonDocument>.Filter.Empty;
        var update = Builders<BsonDocument>.Update.Set("StateName", "Enqueued").Set("Queue", "default");

        collection.UpdateMany(filter, update);
    }

    public static void ConfigureEmailService(this IServiceCollection services)
    {
        services.AddSingleton<ISmtpService, SmtpService>();
        services.AddScoped<IHangfireService, HangfireService>();
    }
    
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "Task manager API", Version = "v1"
            });
        });
    }
    
    public static void AddAuthorizationPolicy(this IServiceCollection services) =>
        services.AddAuthorization(options =>
        {
            options.AddPolicy("User", policy =>
                policy.RequireRole("User")); 
        });
    
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["ValidIssuer"];
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["ValidIssuer"],
                    ValidAudience = jwtSettings["ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
    }

    public static void ConfigureRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
         services.AddHostedService<TaskCreatedConsumer>();
         services.AddHostedService<TaskUpdatedConsumer>();
         services.AddHostedService<TaskDeletedConsumer>();
    }
    
    public static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(
                        new Uri("http://notifications-service:9200"))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = "microservices-logs-{0:yyyy.MM.dd}"
                })
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
        });
    }
}