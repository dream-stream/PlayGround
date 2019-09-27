﻿using System;
using System.Linq;
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
            WebSocketReceiveResult result = null;
            Console.WriteLine($"Handling message {context.Connection.RemoteIpAddress}");
            try
            {
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message =
                        LZ4MessagePackSerializer.Deserialize<BaseMessage>(buffer.Take(result.Count).ToArray());

                    switch (message)
                    {
                        case MessageHeader header:
                            HandleMessage(header);
                            break;
                        case Message msg:
                            HandleMessage(msg);
                            break;
                        default:
                            throw new Exception($"Unknown type: {message.GetType()}");
                    }
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
