using System.Collections.Generic;
using System.Linq;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;

namespace Take.Chat.Core.Services
{
    public class MensagemServico : IMensagemServico
    {

        private string NormalizarConteudoMensagem(string mensagem)
        {
            var conteudoMensagem = mensagem;
            conteudoMensagem = NormalizarConteudoMensagemPrivada(conteudoMensagem);
            conteudoMensagem = NormalizarConteudoMensagemUsuarioDestino(conteudoMensagem);
            return conteudoMensagem;
        }

        private string NormalizarConteudoMensagemPrivada(string mensagem)
        {
            if (PossuiIndicadorMensagemPrivada(mensagem))
                return mensagem.Remove(0, 2).Trim();
            return mensagem;
        }

        private string NormalizarConteudoMensagemUsuarioDestino(string mensagem)
        {
            if (PossuiUsuarioDestino(mensagem))
            {
                var termoUsuarioDestino = mensagem.Split(' ').First();
                return mensagem.Remove(0, termoUsuarioDestino.Length + 1);
            }
            return mensagem;
        }

        private Mensagem ObterMensagemFormatada(Mensagem mensagem)
        {
            var mensagemFormatada = "";

            mensagemFormatada += $"{mensagem.UsuarioOrigem.Apelido} disse";

            if (mensagem.IndicadorMensagemPrivada)
                mensagemFormatada += $" privado";

            if (mensagem.UsuarioDestino != null)
                mensagemFormatada += $" para {mensagem.UsuarioDestino.Apelido}";

            mensagemFormatada += $": {NormalizarConteudoMensagem(mensagem.ConteudoMensagem)}";

            mensagem.ConteudoMensagem = mensagemFormatada;

            return mensagem;
        }

        //---------------------------------

        public IEnumerable<Mensagem> ListarMensagemParaTodos(Usuario usuarioOrigem, string mensagem)
        {
            var mensagens = new List<Mensagem>();

            var mensagemUsarioParaTodos = new Mensagem
            {
                UsuarioOrigem = usuarioOrigem,
                UsuarioDestino = null,
                IndicadorMensagemPrivada = false,
                ConteudoMensagem = mensagem
            };
            mensagemUsarioParaTodos = ObterMensagemFormatada(mensagemUsarioParaTodos);

            var mensagemSistemaParaUsuario = new Mensagem
            {
                UsuarioOrigem = null,
                UsuarioDestino = usuarioOrigem,
                IndicadorMensagemPrivada = false,
                ConteudoMensagem = mensagemUsarioParaTodos.ConteudoMensagem
            };

            mensagens.Add(mensagemSistemaParaUsuario);
            mensagens.Add(mensagemUsarioParaTodos);

            return mensagens;

        }

        public IEnumerable<Mensagem> ListarMensagemParaUsuario(Usuario usuarioOrigem, Usuario usuarioDestino, string mensagem)
        {
            var indicadorMensagemPrivada = PossuiIndicadorMensagemPrivada(mensagem);

            var mensagens = new List<Mensagem>();

            var mensagemUsarioParaUsuario = new Mensagem
            {
                UsuarioOrigem = usuarioOrigem,
                UsuarioDestino = usuarioDestino,
                IndicadorMensagemPrivada = indicadorMensagemPrivada,
                ConteudoMensagem = mensagem
            };
            mensagemUsarioParaUsuario = ObterMensagemFormatada(mensagemUsarioParaUsuario);

            var mensagemSistemaParaUsario = new Mensagem
            {
                UsuarioOrigem = usuarioOrigem,
                UsuarioDestino = usuarioOrigem,
                IndicadorMensagemPrivada = indicadorMensagemPrivada,
                ConteudoMensagem = mensagemUsarioParaUsuario.ConteudoMensagem
            };
            mensagens.Add(mensagemSistemaParaUsario);

            if (!indicadorMensagemPrivada)
                mensagemUsarioParaUsuario.UsuarioDestino = null;

            mensagens.Add(mensagemUsarioParaUsuario);

            return mensagens;
        }

        public IEnumerable<Mensagem> ListarMensagemEntrarSala(Usuario usuario, Sala sala)
        {

            var mensagens = new List<Mensagem>();

            var mensagemSistemaParaUsario = new Mensagem
            {
                UsuarioOrigem = null,
                UsuarioDestino = usuario,
                ConteudoMensagem = $"Você entrou como '{usuario.Apelido}' na sala #{sala.Nome}"
            };

            var mensagemUsarioParaSala = new Mensagem
            {
                UsuarioOrigem = usuario,
                UsuarioDestino = null,
                ConteudoMensagem = $"'{usuario.Apelido}' entrou na sala #{sala.Nome}"
            };

            mensagens.Add(mensagemSistemaParaUsario);
            mensagens.AddRange(ListarMensagemInstrucoes(usuario));
            mensagens.Add(mensagemUsarioParaSala);

            return mensagens;
        }

        public IEnumerable<Mensagem> ListarMensagemInstrucoes(Usuario usuario)
        {
            var instrucoes = new List<string>();
            instrucoes.Add(" ");
            instrucoes.Add("## INSTRUÇÕES ## ");
            instrucoes.Add("Para enviar uma mensagem, escreva sua mensagem e aperte 'ENTER'.");
            instrucoes.Add("Para enviar uma mensagem direcionada para alguém, inicie a mensagem com o apelido precedido de '@': Exemplo: @Fulano Minha mensagem para Fulano.");
            instrucoes.Add("Para enviar uma mensagem privada direcionada para alguém, inicie a mensagem com '/p' seguido do apelido precedido de '@': Exemplo: /p @Fulano Minha mensagem privada para Fulano.");
            instrucoes.Add("Para sair escreve '/sair'");
            instrucoes.Add("Para ver as instruções escreva '/ajuda'");
            instrucoes.Add(" ");
            instrucoes.Add(" ");

            var mensagens = instrucoes.Select(d => new Mensagem
            {
                UsuarioOrigem = null,
                UsuarioDestino = usuario,
                ConteudoMensagem = d
            });

            return mensagens;
        }

        public IEnumerable<Mensagem> ListarMensagemSairSala(Usuario usuario, Sala sala)
        {

            var mensagens = new List<Mensagem>();

            var mensagemSistemaParaUsario = new Mensagem
            {
                UsuarioOrigem = null,
                UsuarioDestino = usuario,
                ConteudoMensagem = $"Você saiu da sala #{sala.Nome}"
            };

            var mensagemUsarioParaSala = new Mensagem
            {
                UsuarioOrigem = usuario,
                UsuarioDestino = null,
                ConteudoMensagem = $"'{usuario.Apelido}' saiu da sala #{sala.Nome}"
            };

            mensagens.Add(mensagemSistemaParaUsario);
            mensagens.Add(mensagemUsarioParaSala);

            return mensagens;
        }

        //---------------------------------

        public bool PossuiComando(string mensagem)
        {
            return PossuiComandoSair(mensagem) || PossuiComandoAjuda(mensagem);
        }

        public bool PossuiComandoSair(string mensagem)
        {
            return mensagem.StartsWith("/sair");
        }

        public bool PossuiComandoAjuda(string mensagem)
        {
            return mensagem.StartsWith("/ajuda");
        }

        public bool PossuiUsuarioDestino(string mensagem)
        {
            return mensagem.StartsWith("/p @") || mensagem.StartsWith("@");
        }

        private bool PossuiIndicadorMensagemPrivada(string mensagem)
        {
            return mensagem.StartsWith("/p");
        }

        public string ObterApelidoUsuarioDestino(string mensagem)
        {
            if (PossuiUsuarioDestino(mensagem))
                return NormalizarConteudoMensagemPrivada(mensagem).Split(' ').First().Remove(0, 1);
            return null;
        }
    }
}
