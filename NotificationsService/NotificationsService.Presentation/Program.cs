using MediatR;
using NotificationsService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5255"); 

ServiceExtensions.ConfigureSerilog(builder);
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidators();
builder.Services.AddAuthorizationPolicy();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureEmailService();
builder.Services.ConfigureHangfire(builder.Configuration);
builder.Services.ConfigureRabbitMq();

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureSwagger();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.ConfigureExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Task manager API");
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();