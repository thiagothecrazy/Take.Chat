using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface IUsuarioServico
    {
        Resultado<Usuario> CriarUsuario(string apelido);
    }
}
