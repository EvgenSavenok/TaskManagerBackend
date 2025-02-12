using Application.Contracts.RepositoryContracts;
using Application.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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
            // s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     In = ParameterLocation.Header,
            //     Description = "Place to add JWT with Bearer",
            //     Name = "Authorization",
            //     Type = SecuritySchemeType.ApiKey,
            //     Scheme = "Bearer"
            // });
            // s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             },
            //             Name = "Bearer",
            //         },
            //         new List<string>()
            //     }
            // });
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
}