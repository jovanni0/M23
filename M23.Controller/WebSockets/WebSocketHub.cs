namespace M23.Controller.WebSockets;

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;



public class WebSocketHub
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();

    public void Register(Guid id, WebSocket socket) =>
        _clients[id] = socket;

    public void Unregister(Guid id) =>
        _clients.TryRemove(id, out _);

    
    public async Task BroadcastAsync(object message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);

        foreach (var (id, socket) in _clients)
        {
            try
            {
                if (socket.State == WebSocketState.Open)
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch
            {
                _clients.TryRemove(id, out _);
            }
        }
    }

    
    public async Task SendAsync(WebSocket socket, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
}