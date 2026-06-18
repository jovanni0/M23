using M23.Controller.Data;
using M23.Controller.Services;
using M23.Controller.WebSockets;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<ProcessState>();
builder.Services.AddSingleton<WebSocketHub>();
builder.Services.AddHostedService<SimulatorClient>();
builder.Services.AddSingleton<SimulatorClient>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.Run();