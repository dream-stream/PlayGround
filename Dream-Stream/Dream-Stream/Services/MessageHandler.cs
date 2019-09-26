using System;
using System.Net.WebSockets;
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
                var message = MessagePackSerializer.Deserialize<BaseMessage>(buffer);

                switch (message)
                {
                    case MessageHeader msgHeader:
                        Console.WriteLine(
                            $"Headers: {nameof(msgHeader.ProducerId)}:{msgHeader.ProducerId}, {nameof(msgHeader.Topic)}:{msgHeader.Topic}, {nameof(msgHeader.Partition)}:{msgHeader.Partition}");
                        break;
                    case Message msg:
                        Console.WriteLine($"Msg: {msg.Msg}");
                        break;
                    default:
                        Console.WriteLine("Arrgghh");
                        break;
                }

                if (result.EndOfMessage)
                    break;


            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
