using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;

namespace Take.Chat.Core.Services
{
    public class BatePapoServico : IBatePapoServico
    {
        private readonly IList<string> _notificacaoes = new List<string>();

        private readonly ISalaServico _salaServico;
        private readonly IUsuarioServico _usuarioServico;
        private readonly BatePapo _batePapo;

        public BatePapoServico(ISalaServico salaServico, IUsuarioServico usuarioServico, BatePapo batePapo)
        {
            _salaServico = salaServico;
            _usuarioServico = usuarioServico;
            _batePapo = batePapo;
        }

        private void AdicionarSala(Sala sala)
        {
            _batePapo.Salas.TryAdd(sala.ID.ToString(), sala);
        }

        private Sala ObterOuAdicionarSala(string nome)
        {
            var sala = _batePapo.Salas.FirstOrDefault(d => d.Value.Nome == nome).Value;
            if (sala == null)
            {
                var salaResultado = _salaServico.CriarSala(nome);

                if (!salaResultado.Sucesso)
                    foreach (var notificacao in salaResultado.Notificacoes)
                        _notificacaoes.Add(notificacao);

                sala = salaResultado.Objeto;
            }

            return sala;
        }

        private Usuario ObterUsuario(string apelido)
        {
            var usuario = _batePapo.Salas.SelectMany(d => d.Value.Usuarios).FirstOrDefault(d => d.Apelido == apelido);

            if (usuario == null)
            {
                var usuarioResultado = _usuarioServico.CriarUsuario(apelido);

                if (!usuarioResultado.Sucesso)
                    foreach (var notificacao in usuarioResultado.Notificacoes)
                        _notificacaoes.Add(notificacao);
                usuario = usuarioResultado.Objeto;
            }
            else
                _notificacaoes.Add("Apelido já esta sendo usado.");

            return usuario;
        }

        private void AdicionarUsuarioSala(Usuario usuario, Sala sala)
        {
            _batePapo.Salas.First(d => d.Value.ID == sala.ID).Value.Usuarios.Add(usuario);
        }

        public Resultado<BatePapo> AdicionarUsuarioSala(string apelido, string nomeSala)
        {
            var sala = ObterOuAdicionarSala(nomeSala);
            var usuario = ObterUsuario(apelido);

            //Verificar Notificações
            if (_notificacaoes.Any())
                return Resultado<BatePapo>.Erro(_notificacaoes);

            AdicionarSala(sala);
            AdicionarUsuarioSala(usuario, sala);

            return Resultado<BatePapo>.Ok(_batePapo);

        }

    }
}
