namespace M23.Tests.Simulator;

using System.Net.Sockets;
using System.Text;
using M23.Shared.Protocol;
using M23.Simulator;
using M23.Simulator.Fsm;
using Xunit;



public class TcpServerTests : IAsyncLifetime
{
    private readonly ProcessOrchestrator _orchestrator = new();
    private readonly TcpServer _server;
    private readonly CancellationTokenSource _cts = new();
    private const int Port = 5100;

    
    public TcpServerTests()
    {
        _server = new TcpServer(_orchestrator);
    }

    
    public Task InitializeAsync()
    {
        _ = _server.StartAsync(Port, _cts.Token);
        return Task.Delay(100);
    }

    
    public Task DisposeAsync()
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }

    
    private static async Task<TcpClient> ConnectAsync()
    {
        var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", Port);
        return client;
    }

    
    private static async Task<string> ReadLineAsync(TcpClient client)
    {
        var buffer = new byte[4096];
        var stream = client.GetStream();
        stream.ReadTimeout = 2000;
        int bytesRead = await stream.ReadAsync(buffer);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    
    private static async Task SendCommandAsync(TcpClient client, string target, string action, string? position = null)
    {
        var message = new SimulatorMessage
        {
            Type = MessageType.Command,
            Payload = System.Text.Json.JsonSerializer.SerializeToElement(new
            {
                target,
                action,
                position
            })
        };
        var bytes = Encoding.UTF8.GetBytes(message.Serialize() + "\n");
        await client.GetStream().WriteAsync(bytes);
        await Task.Delay(100);
    }

    
    [Fact]
    public async Task OnConnect_ReceivesSyncMessage()
    {
        var client = await ConnectAsync();
        var json = await ReadLineAsync(client);
        var message = SimulatorMessage.Deserialize(json);
        Assert.Equal(MessageType.Sync, message?.Type);
        client.Dispose();
    }

    
    [Fact]
    public async Task StartCommand_StartsOutputBelt()
    {
        var client = await ConnectAsync();
        await ReadLineAsync(client);
        await SendCommandAsync(client, "M3", "start");
        Assert.Equal(OutputBeltState.Running, _orchestrator.M3.State);
        client.Dispose();
    }

    
    [Fact]
    public async Task StopCommand_StopsOutputBelt()
    {
        var client = await ConnectAsync();
        await ReadLineAsync(client);
        _orchestrator.StartBelt("M3");
        await SendCommandAsync(client, "M3", "stop");
        Assert.Equal(OutputBeltState.Stopped, _orchestrator.M3.State);
        client.Dispose();
    }

    
    [Fact]
    public async Task S0Command_StopsAllBelts()
    {
        var client = await ConnectAsync();
        await ReadLineAsync(client);
        _orchestrator.StartBelt("M3");
        _orchestrator.StartBelt("M4");
        await SendCommandAsync(client, "S0", "press");
        Assert.Equal(OutputBeltState.Stopped, _orchestrator.M3.State);
        Assert.Equal(OutputBeltState.Stopped, _orchestrator.M4.State);
        client.Dispose();
    }

    
    [Fact]
    public async Task FaultCommand_TransitionsSystemToFault()
    {
        var client = await ConnectAsync();
        await ReadLineAsync(client);
        _orchestrator.StartBelt("M3");
        await SendCommandAsync(client, "FLAP", "fault");
        Assert.Equal(SystemState.Fault, _orchestrator.System.State);
        client.Dispose();
    }

    
    [Fact]
    public async Task FlapSetCommand_MovesFlap()
    {
        var client = await ConnectAsync();
        await ReadLineAsync(client);
        await SendCommandAsync(client, "FLAP", "set", "S6");
        Assert.Equal(FlapPosition.S6, _orchestrator.Flap.Position);
        client.Dispose();
    }
}