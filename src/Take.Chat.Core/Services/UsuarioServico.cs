using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;

namespace Take.Chat.Core.Services
{
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IList<string> _notificacaoes = new List<string>();

        public Resultado<Usuario> CriarUsuario(string apelido)
        {
            var usuario = new Usuario { Apelido = apelido };

            if(UsuarioValido(usuario))
                return Resultado<Usuario>.Ok(usuario);

            return Resultado<Usuario>.Erro(_notificacaoes);
        }

        private bool UsuarioValido(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Apelido))
                _notificacaoes.Add("Apelido de usuário inválido.");

            return !_notificacaoes.Any();
        }
    }
}
