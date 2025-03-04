using System.Text;
using Application.Contracts.Grpc;
using Application.Contracts.MessagingContracts;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TasksService.Infrastructure.Grpc;
using TasksService.Infrastructure.Messaging;
using TasksService.Infrastructure.Redis;
using TasksService.Infrastructure.Repositories;

namespace TasksService.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<ApplicationContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("sqlConnection")));
    
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "Task manager API", Version = "v1"
            });
        });
    }
    
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<TaskValidator>();
        services.AddValidatorsFromAssemblyContaining<TagValidator>();
        services.AddValidatorsFromAssemblyContaining<CommentValidator>();
    }

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
    
    public static void AddAuthorizationPolicy(this IServiceCollection services) =>
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Administrator")); 
            options.AddPolicy("User", policy =>
                policy.RequireRole("User")); 
        });

    public static void AddMessageBrokerServices(this IServiceCollection services)
    {
        services.AddSingleton<ITaskCreatedProducer, TaskCreatedProducer>();
        services.AddSingleton<ITaskUpdatedProducer, TaskUpdatedProducer>();
        services.AddSingleton<ITaskDeletedProducer, TaskDeletedProducer>();
    }

    public static void AddGrpcServices(this IServiceCollection services)
    {
        services.AddScoped<IUserGrpcService, GrpcUserService>();
    }

    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration["Redis:ConnectionString"];
        services.AddSingleton<IRedisCacheService>(_ => new RedisCacheService(redisConnection));
    }
}