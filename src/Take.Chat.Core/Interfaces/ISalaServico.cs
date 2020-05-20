
using System.Collections.Generic;
using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface ISalaServico
    {
        Resultado<Sala> CriarSala(string nome);

        //bool EnviarMensagem(Mensagem mensagem);

        //bool ReceberMensagem(Mensagem mensagem);


        //bool AdicionarUsuario(Sala sala, Usuario usuario);

        //bool RemoverUsuario(Sala sala, Usuario usuario);

    }
}
