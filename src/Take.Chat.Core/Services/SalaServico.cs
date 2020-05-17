using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;

namespace Take.Chat.Core.Services
{
    public class SalaServico : ISalaServico
    {
        private readonly IList<string> _notificacaoes = new List<string>();

        public Resultado<Sala> CriarSala(string nome)
        {
            var objeto = new Sala { Nome = nome };

            if(SalaValida(objeto))
                return Resultado<Sala>.Ok(objeto);

            return Resultado<Sala>.Erro(_notificacaoes);
        }

        private bool SalaValida(Sala sala)
        {
            if (string.IsNullOrWhiteSpace(sala.Nome))
                _notificacaoes.Add("Nome inválido.");

            return !_notificacaoes.Any();
        }
    }
}
