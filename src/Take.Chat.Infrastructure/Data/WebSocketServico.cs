using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Take.Chat.Infrastructure.Data
{
    public class WebSocketServico
    {


        public static async Task<WebSocket> ObterWebSocketAsync(HttpContext context)
        {
            WebSocket webSocket = null;

            if (context.WebSockets.IsWebSocketRequest)
                webSocket = await context.WebSockets.AcceptWebSocketAsync();

            return webSocket;
        }


        public async Task Invoke(WebSocket webSocket)
        {
            CancellationToken ct = default;

            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;

                
                var mensagem = await ReceberMensagemAsync(webSocket, ct);
                
                if (string.IsNullOrEmpty(mensagem))
                {
                    if (webSocket.State != WebSocketState.Open)
                        break;

                    continue;
                }

                await EnviarMensagemAsync(webSocket, mensagem, ct);

            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            webSocket.Dispose();
        }



        public static Task EnviarMensagemAsync(WebSocket socket, string mensagem, CancellationToken ct = default)
        {
            var buffer = Encoding.UTF8.GetBytes(mensagem);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        public static async Task<string> ReceberMensagemAsync(WebSocket socket, CancellationToken ct = default)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
