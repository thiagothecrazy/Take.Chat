using System;
using System.Collections.Generic;
using System.Text;

namespace Take.Chat.Core.Entities
{
    public class Sala : EntidadeBase
    {
        public string Nome { get; set; }

        public IList<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
