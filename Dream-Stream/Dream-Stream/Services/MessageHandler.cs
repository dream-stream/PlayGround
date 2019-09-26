using System;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using Dream_Stream.Models.Messages;
using MessagePack;
using Microsoft.AspNetCore.Http;

namespace Dream_Stream.Services
{
    public class MessageHandler
    {
        public async Task Handle(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (!result.EndOfMessage)
                {
                    var message = LZ4MessagePackSerializer.Deserialize<MessageHeader>(buffer);
                    HandleMessage(message);
                }
                else
                {
                    var message = LZ4MessagePackSerializer.Deserialize<Message>(buffer.Take(result.Count).ToArray());
                    HandleMessage(message);
                }


            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
        }

        private void HandleMessage(MessageHeader message)
        {
            Console.WriteLine(
                        $"Headers: {nameof(message.ProducerId)}:{message.ProducerId}, {nameof(message.Topic)}:{message.Topic}, {nameof(message.Partition)}:{message.Partition}");
        }

        private void HandleMessage(Message message)
        {
            Console.WriteLine($"Msg: {message.Msg}");
        }
    }
}
