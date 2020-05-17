
using Take.Chat.Core.Entities;

namespace Take.Chat.Core.Interfaces
{
    public interface IBatePapo
    {
        BatePapo CriarBatePapo();

        bool AdicionarSala(Sala sala);
    }
}
