using MediatR;
using UsersService.Application.Contracts;
using UsersService.Application.MappingProfiles;
using UsersService.Infrastructure;
using UsersService.Infrastructure.Extensions;
using UsersService.Presentation.SignalRHubs;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5151"); 

builder.Services.AddSignalR();
ServiceExtensions.ConfigureSerilog(builder);
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureIdentity();
builder.Services.AddAuthorizationPolicy();
builder.Services.ConfigureJwt(builder.Configuration);
ServiceExtensions.ConfigureCors(builder);
builder.Services.AddCookies();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureSwagger();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseCors("UsersPolicy");

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Task manager API");
});

app.UseRouting();

app.ConfigureExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapHub<UserHub>("/userHub");

app.ApplyMigrations();

app.Run();