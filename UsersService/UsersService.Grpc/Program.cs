using MediatR;
using UsersService.Application.Contracts;
using UsersService.Application.MappingProfiles;
using UsersService.Application.UseCases.Queries.UserQueries.GetUserById;
using UsersService.Grpc.Services;
using UsersService.Infrastructure;
using UsersService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5220"); 

builder.Services.ConfigureSqlContext(builder.Configuration);

builder.Services.AddAutoMapper(typeof(UserMappingProfile));

builder.Services.AddMediatR(typeof(GetUserByIdQueryHandler).Assembly);

builder.Services.ConfigureIdentity();

builder.Services.AddDataProtection();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UserGrpcService>();

app.Run();