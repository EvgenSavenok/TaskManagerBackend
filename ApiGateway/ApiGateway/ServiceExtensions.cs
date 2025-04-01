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
            
            options.AddPolicy("SignalRPolicy", policy =>
            {
                policy.WithOrigins(
                    // Angular
                    "http://localhost:4201",
                    "http://angular-frontend:4201", 
                    // API Gateway
                    "http://api-gateway:5271", 
                    // Users Service
                    "http://localhost:5151", 
                    "http://users-service:5151", 
                    // Notifications Service
                    "http://localhost:5255",
                    "http://notifications-service:5255",
                    // Tasks Service
                    "http://localhost:5022", 
                    "http://tasks-service:5022" 
                );
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
                policy.AllowCredentials();
            });
        });
    }
}