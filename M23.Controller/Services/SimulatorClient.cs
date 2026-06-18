namespace M23.Controller.Services;

using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using M23.Controller.Data;
using M23.Controller.WebSockets;
using M23.Shared.Protocol;
using Microsoft.EntityFrameworkCore;



public class SimulatorClient : BackgroundService
{
    private readonly ProcessState _state;
    private readonly WebSocketHub _hub;
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<SimulatorClient> _logger;

    
    public SimulatorClient(
        ProcessState state,
        WebSocketHub hub,
        IDbContextFactory<AppDbContext> dbFactory,
        IConfiguration config,
        ILogger<SimulatorClient> logger)
    {
        _state = state;
        _hub = hub;
        _dbFactory = dbFactory;
        _config = config;
        _logger = logger;
    }

    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var host = _config["Simulator:Host"] ?? "127.0.0.1";
        var port = int.Parse(_config["Simulator:Port"] ?? "5000");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(host, port, stoppingToken);
                _logger.LogInformation("Connected to simulator at {Host}:{Port}", host, port);

                await ReadLoopAsync(client, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Simulator connection lost: {Message}. Retrying in 3s.", ex.Message);
                await Task.Delay(3000, stoppingToken);
            }
        }
    }

    
    private async Task ReadLoopAsync(TcpClient client, CancellationToken cancellationToken)
    {
        var stream = client.GetStream();
        var buffer = new byte[4096];
        var leftover = string.Empty;

        while (!cancellationToken.IsCancellationRequested && client.Connected)
        {
            int bytesRead = await stream.ReadAsync(buffer, cancellationToken);
            if (bytesRead == 0) break;

            var chunk = leftover + Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var lines = chunk.Split('\n');

            // last element may be incomplete — carry it over
            leftover = lines[^1];

            foreach (var line in lines[..^1])
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                await HandleMessageAsync(line);
            }
        }
    }

    
    private async Task HandleMessageAsync(string json)
    {
        var message = SimulatorMessage.Deserialize(json);
        if (message == null) return;

        if (message.Type == MessageType.Event)
        {
            var source = message.Payload.GetProperty("source").GetString() ?? string.Empty;
            var from   = message.Payload.GetProperty("from").GetString() ?? string.Empty;
            var to     = message.Payload.GetProperty("to").GetString() ?? string.Empty;

            _state.Apply(source, to);

            await _hub.BroadcastAsync(new
            {
                type = source switch
                {
                    "SYSTEM" => "system_state",
                    "FLAP"   => "flap_state",
                    "ALARM"  => "alarm",
                    _        => "belt_state"
                },
                payload = source switch
                {
                    "SYSTEM" => (object)new { state = to },
                    "FLAP"   => new { position = to },
                    "ALARM"  => new { active = to == "Active" },
                    _        => new { belt = source, state = to }
                }
            });

            await LogEventAsync(source, from, to, message.Timestamp);
        }
    }

    
    private async Task LogEventAsync(string source, string from, string to, DateTime timestamp)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            db.ProcessEvents.Add(new ProcessEventEntity
            {
                Source    = source,
                From      = from,
                To        = to,
                Timestamp = timestamp
            });
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log event.");
        }
    }

    
    public async Task SendCommandAsync(string json)
    {
        
    }
}