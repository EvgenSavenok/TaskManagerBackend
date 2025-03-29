using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using UsersService.Application.Contracts;
using UsersService.Application.MappingProfiles;
using UsersService.Infrastructure;
using UsersService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

ServiceExtensions.ConfigureSerilog(builder);
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureIdentity();
builder.Services.AddAuthorizationPolicy();
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("UsersPolicy", b =>
        b.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); 
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "refreshToken";
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.ConfigureSwagger();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//app.UseCors("UsersPolicy");

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

app.Run();