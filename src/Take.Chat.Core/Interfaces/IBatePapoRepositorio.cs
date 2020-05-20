using System;
using System.Collections.Generic;
using System.Text;
using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface IBatePapoRepositorio
    {
        BatePapo ObterBatePapo();

        Sala ObterSalaPeloNome(string nome);

        Sala ObterSalaPeloUsuario(Usuario usuario);

        void AdicionarSala(Sala sala);

        Usuario ObterUsuarioPeloApelido(string apelido);

        Usuario ObterUsuarioPeloID(string apelido);

        void AdicionarUsuarioSala(Usuario usuario, Sala sala);

        void RemoverUsuarioSala(Usuario usuario);

        void EnviarMensagemAsync(Usuario usuario, string mensagem);

    }
}
