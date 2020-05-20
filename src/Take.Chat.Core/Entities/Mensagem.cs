using System;
using System.Collections.Generic;
using System.Text;

namespace Take.Chat.Core.Entities
{
    public class Mensagem : EntidadeBase
    {
        public Usuario UsuarioOrigem { get; set; }

        public Usuario UsuarioDestino { get; set; }

        public bool IndicadorMensagemPrivada { get; set; }

        public string ConteudoMensagem { get; set; }
    }
}
