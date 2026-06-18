namespace M23.Controller.WebSockets;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using M23.Controller.Services;
using M23.Shared.Protocol;



public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;

    
    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    
    public async Task InvokeAsync(
        HttpContext context,
        WebSocketHub hub,
        ProcessState state,
        SimulatorClient simulatorClient)
    {
        if (context.Request.Path != "/ws")
        {
            await _next(context);
            return;
        }

        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var id = Guid.NewGuid();
        hub.Register(id, socket);

        await hub.SendAsync(socket, new
        {
            type = "sync",
            payload = state.Snapshot()
        });

        await ReceiveLoopAsync(socket, simulatorClient);

        hub.Unregister(id);
    }

    
    private static async Task ReceiveLoopAsync(WebSocket socket, SimulatorClient simulatorClient)
    {
        var buffer = new byte[4096];

        while (socket.State == WebSocketState.Open)
        {
            try
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    break;
                }

                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await simulatorClient.SendCommandAsync(json);
            }
            catch
            {
                break;
            }
        }
    }
}