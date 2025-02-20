using MediatR;
using UsersService.Application.Contracts;
using UsersService.Application.MappingProfiles;
using UsersService.Infrastructure;
using UsersService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureIdentity();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseRouting();

app.ConfigureExceptionHandler();

app.MapControllers();
app.MapRazorPages();

app.Run();