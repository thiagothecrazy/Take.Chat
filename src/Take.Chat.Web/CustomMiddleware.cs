using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Take.Chat.Core.Interfaces;
using Take.Chat.Web.Models;

namespace Take.Chat.Infrastructure.Data
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketRepositorio _webSocketRepositorio;
        private IBatePapoServico _batePapoServico;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
            
        }

        public async Task Invoke(HttpContext context, WebSocketRepositorio webSocketRepositorio, IBatePapoServico batePapoServico)
        {
            _webSocketRepositorio = webSocketRepositorio;
            _batePapoServico = batePapoServico;

            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            await ProcessarAsync(context);
        }

        //------------------

        public async Task<WebSocket> ObterWebSocketAsync(HttpContext context)
        {
            WebSocket webSocket = null;
            if (context.WebSockets.IsWebSocketRequest)
                webSocket = await context.WebSockets.AcceptWebSocketAsync();
            return webSocket;
        }

        public async Task ProcessarAsync(HttpContext context)
        {
            var webSocket = await ObterWebSocketAsync(context);
            if (webSocket == null)
                return;

            var webSocketID = ObterWebSocketID(webSocket);
            CancellationToken ct = default;

            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;

                //Recuperar mensagem WebSocket
                var strMensagem = await _webSocketRepositorio.ReceberMensagemAsync(webSocket, ct);
                if (string.IsNullOrEmpty(strMensagem))
                {
                    if (webSocket.State != WebSocketState.Open)
                        break;
                    continue;
                }

                var webSocketDTO = JsonConvert.DeserializeObject<MensagemWebSocketDTO>(strMensagem);
                
                //Verificar WebSocket: Vincular WebSocket com Usuario
                if (string.IsNullOrEmpty(webSocketID))
                    webSocketID = VerificarWebSocketUsuario(webSocket, webSocketDTO.UsuarioID);

                //Processar mensagem
                if(!string.IsNullOrEmpty(webSocketDTO.Mensagem))
                    _batePapoServico.ProcessarMensagem(webSocketDTO.UsuarioID, webSocketDTO.Mensagem);
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", ct);
            webSocket.Dispose();
        }

        private string VerificarWebSocketUsuario(WebSocket webSocket, string usuarioID)
        {
            var webSocketID = ObterWebSocketID(webSocket);

            if (string.IsNullOrEmpty(webSocketID) || usuarioID != webSocketID)
            {
                _webSocketRepositorio.AdicionarOuAlterarSocket(usuarioID, webSocket);
                _batePapoServico.EntrarSala(usuarioID);
            }

            return usuarioID;
        }

        private string ObterWebSocketID(WebSocket webSocket)
        {
            return _webSocketRepositorio.ObterWebSocketID(webSocket);
        }

    }
}
