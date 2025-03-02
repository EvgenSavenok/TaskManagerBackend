namespace ApiGateway;

public static class ServiceExtensions
{
    public static void AddPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("UsersPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:5151");    
                policy.AllowAnyMethod(); 
                policy.AllowAnyHeader();    
            });
            options.AddPolicy("TasksPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:5022");
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
            options.AddPolicy("NotificationsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:5255");
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
            options.AddPolicy("GrpcPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:5220");
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
        });
    }
}