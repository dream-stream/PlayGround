using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Dream_Stream.Models.Messages;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace Dream_Stream.Services
{
    public class MessageHandler
    {
        private static readonly Counter Counter = Metrics.CreateCounter("Messages_Received", "");

        public async Task Handle(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = null;
            Console.WriteLine($"Handling message from: {context.Connection.RemoteIpAddress}");
            try
            {
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.CloseStatus.HasValue) break;

                    var message =
                        LZ4MessagePackSerializer.Deserialize<MessageContainer>(buffer.Take(result.Count).ToArray());
                    HandleMessage(message);
                    
                } while (!result.CloseStatus.HasValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, result?.CloseStatusDescription ?? "Failed hard", CancellationToken.None);
            }
        }

        private void HandleMessage(MessageContainer messages)
        {
            messages.Print();
        }
    }
}
