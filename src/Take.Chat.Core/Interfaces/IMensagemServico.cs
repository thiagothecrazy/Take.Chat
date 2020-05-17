
using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface IMensagemServico
    {
        Mensagem CriarMensagem(string conteudo);

        bool ValidarMensagem(Mensagem mensagem);
    }
}
