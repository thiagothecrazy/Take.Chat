using System.Collections.Generic;

namespace Take.Chat.Core.Entities
{
    public class BatePapo : EntidadeBase
    {
        public IEnumerable<Sala> Salas { get; set; }
    }
}
