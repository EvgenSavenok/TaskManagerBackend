using MediatR;
using UsersService.Application.MappingProfiles;
using UsersService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSqlContext(builder.Configuration);
// Why i cannot use AppDomain.CurrentDomain.GetAssemblies()?
// It cannot find mapping without typeof(UserMappingProfile)
// Need to find answer tomorrow
// TODO
builder.Services.AddAutoMapper(typeof(UserMappingProfile));
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureIdentity();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseRouting();

app.ConfigureExceptionHandler();

app.MapControllers();
app.MapRazorPages();

app.Run();