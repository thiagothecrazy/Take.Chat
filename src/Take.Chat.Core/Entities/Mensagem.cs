using System;
using System.Collections.Generic;
using System.Text;

namespace Take.Chat.Core.Entities
{
    public class Mensagem : EntidadeBase
    {
        public Guid SalaID { get; set; }

        public Guid UsuarioOrigemID { get; set; }

        public Guid UsuarioDestinoID { get; set; }

        public bool MensagemPrivada { get; set; }

        public string ConteudoMensagem { get; set; }
    }
}
