using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Take.Chat.Infrastructure.Data
{
    public class WebSocketRepositorio : IWebSocketRepositorio
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket ObterWebSocket(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> ListarTodos()
        {
            return _sockets;
        }

        public string ObterWebSocketID(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public void AdicionarOuAlterarSocket(string id, WebSocket socket)
        {
            if (_sockets.TryGetValue(id, out WebSocket currentSocket))
                _sockets.TryUpdate(id, socket, currentSocket);
            else
                _sockets.TryAdd(id, socket);
        }

        public void RemoverSocketAsync(string id)
        {
            _sockets.TryRemove(id, out WebSocket socket);

            socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                   statusDescription: "Closed",
                                   cancellationToken: CancellationToken.None);
        }

        //------------------------------

        public async Task EnviarMensagemAsync(string socketId, string mensagem, CancellationToken ct = default)
        {
            var socket = ObterWebSocket(socketId);
            await EnviarMensagemAsync(socket, mensagem, ct);
        }

        public async Task EnviarMensagemAsync(WebSocket socket, string mensagem, CancellationToken ct = default)
        {
            if (socket == null || socket.State != WebSocketState.Open)
                return;

            var buffer = Encoding.UTF8.GetBytes(mensagem);
            var segment = new ArraySegment<byte>(buffer);

            await socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        public async Task<string> ReceberMensagemAsync(WebSocket socket, CancellationToken ct = default)
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
