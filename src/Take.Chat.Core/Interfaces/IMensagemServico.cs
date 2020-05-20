
using System.Collections.Generic;
using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface IMensagemServico
    {
        IEnumerable<Mensagem> ListarMensagemParaTodos(Usuario usuarioOrigem, string mensagem);

        IEnumerable<Mensagem> ListarMensagemParaUsuario(Usuario usuarioOrigem, Usuario usuarioDestino, string mensagem);

        IEnumerable<Mensagem> ListarMensagemEntrarSala(Usuario usuario, Sala sala);

        IEnumerable<Mensagem> ListarMensagemInstrucoes(Usuario usuario);

        IEnumerable<Mensagem> ListarMensagemSairSala(Usuario usuario, Sala sala);

        bool PossuiComando(string mensagem);

        bool PossuiComandoSair(string mensagem);

        bool PossuiComandoAjuda(string mensagem);

        bool PossuiUsuarioDestino(string mensagem);

        string ObterApelidoUsuarioDestino(string mensagem);
    }
}
