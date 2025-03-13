using ApiGateway;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPolicies();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

var app = builder.Build();

// app.UseCors("UsersPolicy");
// app.UseCors("TasksPolicy");
//app.UseCors("NotificationsPolicy");
app.UseCors("SignalRPolicy");

app.UseRouting();

app.UseOcelot().Wait();

app.Run();