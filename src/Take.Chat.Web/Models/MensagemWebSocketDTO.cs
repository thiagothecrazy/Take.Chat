using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take.Chat.Web.Models
{
    public class MensagemWebSocketDTO
    {
        public string UsuarioID { get; set; }

        public string Mensagem { get; set; }
    }
}
