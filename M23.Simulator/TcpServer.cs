using M23.Simulator.Fsm;

namespace M23.Simulator;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using M23.Shared.Protocol;




public class TcpServer
{
    private readonly ProcessOrchestrator _orchestrator;
    private readonly List<TcpClient> _clients = new();
    private readonly object _lock = new();
    private TcpListener? _listener;

    
    public TcpServer(ProcessOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
        _orchestrator.EventOccurred += OnEventOccurred;
    }

    
    public async Task StartAsync(int port, CancellationToken cancellationToken)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        Console.WriteLine($"TCP server listening on port {port}");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                Console.WriteLine("Client connected.");
                _ = HandleClientAsync(client, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _listener.Stop();
    }

    
    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        lock (_lock) _clients.Add(client);

        try
        {
            await SendMessageAsync(client, BuildSyncMessage());

            var stream = client.GetStream();
            var buffer = new byte[4096];

            while (!cancellationToken.IsCancellationRequested && client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0) break;

                var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                HandleIncoming(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
        finally
        {
            lock (_lock) _clients.Remove(client);
            client.Dispose();
            Console.WriteLine("Client disconnected.");
        }
    }

    
    /*
     * Handle incoming message.
     */
    private void HandleIncoming(string json)
    {
        var message = SimulatorMessage.Deserialize(json);
        
        if (message?.Type != MessageType.Command) 
            return;

        var payload = message.Payload.Deserialize<CommandPayload>();
        
        if (payload == null) 
            return;

        switch (payload.Action)
        {
            case "start":
                _orchestrator.StartBelt(payload.Target);
                break;
            case "stop":
                _orchestrator.StopBelt(payload.Target);
                break;
            case "press" when payload.Target == "S0":
                _orchestrator.StopAllBelts();
                break;
            case "press" when payload.Target == "S5":
                _orchestrator.StopInputBelts();
                break;
            case "set" when payload.Target == "FLAP":
                if (Enum.TryParse<FlapPosition>(payload.Position, out var pos))
                    _orchestrator.MoveFlap(pos);
                break;
            case "fault":
                _orchestrator.TriggerFault();
                break;
            case "restart":
                _orchestrator.Restart();
                break;
        }
    }

    
    /*
     * When an event occurs, broadcast to all connections.
     */
    private void OnEventOccurred(ProcessEvent e)
    {
        var message = SimulatorMessage.CreateEvent(e.Source, e.From, e.To);
        message.Timestamp = e.Timestamp;
        BroadcastAsync(message).GetAwaiter().GetResult();
    }

    
    /*
     * Broadcast a message to all connections.
     */
    private async Task BroadcastAsync(SimulatorMessage message)
    {
        List<TcpClient> snapshot;
        lock (_lock) snapshot = new List<TcpClient>(_clients);

        foreach (var client in snapshot)
        {
            try
            {
                await SendMessageAsync(client, message);
            }
            catch
            {
                lock (_lock) _clients.Remove(client);
            }
        }
    }
    

    private static async Task SendMessageAsync(TcpClient client, SimulatorMessage message)
    {
        var json = message.Serialize() + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);
        await client.GetStream().WriteAsync(bytes);
    }

    
    private SimulatorMessage BuildSyncMessage() =>
        SimulatorMessage.CreateSync(
            _orchestrator.System.State.ToString(),
            _orchestrator.Flap.Position.ToString(),
            _orchestrator.M1.State.ToString(),
            _orchestrator.M2.State.ToString(),
            _orchestrator.M3.State.ToString(),
            _orchestrator.M4.State.ToString());
}