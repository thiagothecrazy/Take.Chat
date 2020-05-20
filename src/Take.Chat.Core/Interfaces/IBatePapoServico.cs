using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface IBatePapoServico
    {
        Resultado<BatePapo> AdicionarUsuarioSala(string apelido, string nomeSala);

        Usuario ObterUsuario(string apelido);

        bool EntrarSala(string usuarioID);

        bool ProcessarMensagem(string usuarioID, string mensagem);
    }
}
