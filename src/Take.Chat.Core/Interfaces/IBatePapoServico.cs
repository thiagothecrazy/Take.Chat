using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface IBatePapoServico
    {
        Resultado<BatePapo> AdicionarUsuarioSala(string apelido, string nomeSala);
    }
}
