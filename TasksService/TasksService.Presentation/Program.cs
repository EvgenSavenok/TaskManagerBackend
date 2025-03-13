using MediatR;
using TasksService.Infrastructure.Extensions;
using TasksService.Infrastructure.Grpc;
using TasksService.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

ServiceExtensions.ConfigureSerilog(builder);
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidators();
builder.Services.AddAuthorizationPolicy();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.AddMessageBrokerServices();
builder.Services.AddGrpcServices();

builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureSwagger();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Task manager API");
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureExceptionHandler();

app.MapControllers();
app.MapRazorPages();

app.MapGrpcService<GrpcUserService>();

app.MapHub<TaskHub>("/taskHub").AllowAnonymous();

app.Run();