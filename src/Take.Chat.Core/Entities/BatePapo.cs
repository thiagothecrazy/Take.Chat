using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Take.Chat.Core.Entities
{
    public class BatePapo : EntidadeBase
    {
        public BatePapo()
        {
            Salas = new ConcurrentDictionary<string, Sala>();
        }

        public ConcurrentDictionary<string, Sala> Salas { get; set; }

    }
}
