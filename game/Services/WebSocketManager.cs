using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace game.Services;

public class WebSocketManager
{
    public WebSocketManager() { }
    
    private readonly ConcurrentBag<WebSocket> _clients = new();

    public async Task HandleConnection(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        _clients.Add(webSocket);

        Console.WriteLine("Клиент подключён");

        var buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Получено: {message}");

                // 🔁 Отправка эхо-ответа текущему клиенту:
                byte[] echo = Encoding.UTF8.GetBytes("echo: " + message);
                await webSocket.SendAsync(new ArraySegment<byte>(echo), WebSocketMessageType.Text, true, CancellationToken.None);

                await BroadcastMessage(message, webSocket);
            }

        }
        
        

        _clients.TryTake(out _); // Удалить отключившегося клиента
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрыто", CancellationToken.None);
        Console.WriteLine("Клиент отключился");
    }

    private async Task BroadcastMessage(string message, WebSocket sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(data);

        foreach (var client in _clients)
        {
            if (client != sender && client.State == WebSocketState.Open)
            {
                await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}