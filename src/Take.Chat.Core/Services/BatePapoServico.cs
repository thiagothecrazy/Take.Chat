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
        private readonly IMensagemServico _mensagemServico;
        private readonly IBatePapoRepositorio _batePapoRepositorio;

        public BatePapoServico(ISalaServico salaServico, IUsuarioServico usuarioServico, IBatePapoRepositorio batePapoRepositorio, IMensagemServico mensagemServico)
        {
            _salaServico = salaServico;
            _usuarioServico = usuarioServico;
            _batePapoRepositorio = batePapoRepositorio;
            _mensagemServico = mensagemServico;
        }

        private void AdicionarSala(Sala sala)
        {
            _batePapoRepositorio.AdicionarSala(sala);
        }

        private Sala ObterOuCriarSala(string nome)
        {
            //Recuperar sala pelo nome
            var sala = _batePapoRepositorio.ObterSalaPeloNome(nome);

            //Se a sala não existir, criar uma nova
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

        public Usuario ObterUsuario(string apelido)
        {
            var usuario = _batePapoRepositorio.ObterUsuarioPeloApelido(apelido);

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
            _batePapoRepositorio.AdicionarUsuarioSala(usuario, sala);
        }

        public Resultado<BatePapo> AdicionarUsuarioSala(string apelido, string nomeSala)
        {
            var sala = ObterOuCriarSala(nomeSala);
            var usuario = ObterUsuario(apelido);

            //Verificar Notificações
            if (_notificacaoes.Any())
                return Resultado<BatePapo>.Erro(_notificacaoes);

            AdicionarSala(sala);
            AdicionarUsuarioSala(usuario, sala);

            var batePapo = _batePapoRepositorio.ObterBatePapo();
            return Resultado<BatePapo>.Ok(batePapo);

        }

        //------------------------------

        public void Ajuda(Usuario usuario)
        {
            var mensagens = _mensagemServico.ListarMensagemInstrucoes(usuario);
            EnviarMensagem(mensagens);
        }

        public void EntrarSala(string usuarioID)
        {
            var usuario = _batePapoRepositorio.ObterUsuarioPeloID(usuarioID);
            var sala = _batePapoRepositorio.ObterSalaPeloUsuario(usuario);

            var mensagens = _mensagemServico.ListarMensagemEntrarSala(usuario, sala);
            EnviarMensagem(mensagens);
        }

        public void SairSala(Usuario usuario)
        {
            var sala = _batePapoRepositorio.ObterSalaPeloUsuario(usuario);

            var mensagens = _mensagemServico.ListarMensagemSairSala(usuario, sala);
            EnviarMensagem(mensagens);

            //REMOVER USUÁRIOS
            _batePapoRepositorio.RemoverUsuarioSala(usuario);

        }

        //------------------------------

        private bool ProcessarMensagemComando(Usuario usuarioOrigem, string mensagem)
        {
            var possuiComando = _mensagemServico.PossuiComando(mensagem);

            if (possuiComando)
            {
                if (_mensagemServico.PossuiComandoSair(mensagem))
                    SairSala(usuarioOrigem);
                else if (_mensagemServico.PossuiComandoAjuda(mensagem))
                    Ajuda(usuarioOrigem);
            }

            return possuiComando;
        }

        private bool ProcessarMensagemParaUsuario(Usuario usuarioOrigem, string mensagem)
        {
            var possuiUsuarioDestino = _mensagemServico.PossuiUsuarioDestino(mensagem);

            if (possuiUsuarioDestino)
            {
                var apelidoUsuarioDestino = _mensagemServico.ObterApelidoUsuarioDestino(mensagem);
                var usuarioDestino = _batePapoRepositorio.ObterUsuarioPeloApelido(apelidoUsuarioDestino);

                if (usuarioDestino == null)
                    return false;

                var mensagens = _mensagemServico.ListarMensagemParaUsuario(usuarioOrigem, usuarioDestino, mensagem);
                EnviarMensagem(mensagens);
            }

            return possuiUsuarioDestino;
        }

        private void ProcessarMensagemParaTodos(Usuario usuarioOrigem, string mensagem)
        {
            var mensagens = _mensagemServico.ListarMensagemParaTodos(usuarioOrigem, mensagem);
            EnviarMensagem(mensagens);
        }

        public void ProcessarMensagem(string usuarioID, string mensagem)
        {
            var conteudoMensagem = mensagem.Trim();
            var usuarioOrigem = _batePapoRepositorio.ObterUsuarioPeloID(usuarioID);

            if (ProcessarMensagemComando(usuarioOrigem, conteudoMensagem))
                return;

            if (ProcessarMensagemParaUsuario(usuarioOrigem, conteudoMensagem))
                return;

            ProcessarMensagemParaTodos(usuarioOrigem, conteudoMensagem);
        }

        //---------------------------

        private void EnviarMensagem(IEnumerable<Mensagem> mensagens)
        {
            foreach (var mensagem in mensagens)
                EnviarMensagem(mensagem);
        }

        private void EnviarMensagem(Mensagem mensagem)
        {
            if (mensagem.UsuarioDestino == null)
                EnviarMensagemTodos(mensagem);
            else
                EnviarMensagemUsuario(mensagem);
        }

        private void EnviarMensagemUsuario(Mensagem mensagem)
        {
            _batePapoRepositorio.EnviarMensagemAsync(mensagem.UsuarioDestino, mensagem.ConteudoMensagem);
        }

        private void EnviarMensagemTodos(Mensagem mensagem)
        {
            var sala = _batePapoRepositorio.ObterSalaPeloUsuario(mensagem.UsuarioOrigem);

            foreach (var usuarioDestino in sala.Usuarios.Where(d => d.ID != mensagem.UsuarioOrigem.ID))
                _batePapoRepositorio.EnviarMensagemAsync(usuarioDestino, mensagem.ConteudoMensagem);
        }


    }
}
