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
    public interface IWebSocketRepositorio
    {
        WebSocket ObterWebSocket(string id);

        ConcurrentDictionary<string, WebSocket> ListarTodos();

        string ObterWebSocketID(WebSocket socket);

        void AdicionarOuAlterarSocket(string id, WebSocket socket);

        void RemoverSocketAsync(string id);


        Task EnviarMensagemAsync(string socketId, string mensagem, CancellationToken ct = default);

        Task EnviarMensagemAsync(WebSocket socket, string mensagem, CancellationToken ct = default);

        Task<string> ReceberMensagemAsync(WebSocket socket, CancellationToken ct = default);
    }
}
