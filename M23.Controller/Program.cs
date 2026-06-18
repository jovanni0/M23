using M23.Controller.Data;
using M23.Controller.Services;
using M23.Controller.WebSockets;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<ProcessState>();
builder.Services.AddSingleton<WebSocketHub>();
builder.Services.AddSingleton<SimulatorClient>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<SimulatorClient>());
builder.Services.AddScoped<ReportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.MapGet("/reports/events", async (
    ReportService reports,
    DateTime? from,
    DateTime? to,
    string? source) =>
{
    var events = await reports.GetEventsAsync(from, to, source);
    return Results.Ok(events);
});

app.MapGet("/reports/faults", async (ReportService reports) =>
{
    var periods = await reports.GetFaultPeriodsAsync();
    return Results.Ok(periods);
});

app.Run();