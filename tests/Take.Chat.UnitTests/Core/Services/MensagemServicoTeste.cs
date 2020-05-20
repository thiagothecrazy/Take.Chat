using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Take.Chat.Core;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;
using Take.Chat.Core.Services;
using Xunit;

namespace Take.Chat.UnitTests.Core.Services
{
    public class MensagemServicoTeste
    {
        private IMensagemServico ObterIMensagemServico()
        {
            return new MensagemServico();
        }

        [Fact]
        public void ListarMensagemParaTodos_MensagemComum_ListaMensagens()
        {
            // Arrange
            var usuario = new Usuario { Apelido = "Thiago" };
            var mensagem = "Olá galera";
            var mensagemFormatada = $"Thiago disse: Olá galera";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ListarMensagemParaTodos(usuario, mensagem);

            // Assert
            Assert.Equal(2, actual.ToList().Count);
            Assert.True(actual.All(d => d.IndicadorMensagemPrivada == false));
            Assert.Single(actual.Where(d => d.UsuarioDestino == null && d.UsuarioOrigem?.ID == usuario.ID));
            Assert.Single(actual.Where(d => d.UsuarioOrigem == null && d.UsuarioDestino?.ID == usuario.ID));
            Assert.True(actual.All(d => d.ConteudoMensagem == mensagemFormatada));
        }

        [Fact]
        public void ListarMensagemParaUsuario_MensagemComumParaUsuario_ListaMensagens()
        {
            // Arrange
            var usuarioOrigem = new Usuario { Apelido = "Thiago" };
            var usuarioDestino = new Usuario { Apelido = "Francine" };
            var mensagem = $"@Francine Oi, tudo bem?";
            var mensagemFormatada = $"Thiago disse para Francine: Oi, tudo bem?";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ListarMensagemParaUsuario(usuarioOrigem, usuarioDestino, mensagem);

            // Assert
            Assert.Equal(2, actual.ToList().Count);
            Assert.True(actual.All(d => d.IndicadorMensagemPrivada == false));
            Assert.Equal(2, actual.Count(d => d.UsuarioOrigem.ID == usuarioOrigem.ID));
            Assert.Single(actual.Where(d => d.UsuarioDestino == null));
            Assert.True(actual.All(d => d.ConteudoMensagem == mensagemFormatada));
        }

        [Fact]
        public void ListarMensagemParaUsuario_MensagemPrivadaParaUsuario_ListaMensagens()
        {
            // Arrange
            var usuarioOrigem = new Usuario { Apelido = "Thiago" };
            var usuarioDestino = new Usuario { Apelido = "Francine" };
            var mensagem = $"/p @Francine Oi, tudo bem?";
            var mensagemFormatada = $"Thiago disse privado para Francine: Oi, tudo bem?";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ListarMensagemParaUsuario(usuarioOrigem, usuarioDestino, mensagem);

            // Assert
            Assert.Equal(2, actual.ToList().Count);
            Assert.True(actual.All(d => d.IndicadorMensagemPrivada == true));
            Assert.Equal(2, actual.Count(d => d.UsuarioOrigem.ID == usuarioOrigem.ID));
            Assert.Single(actual.Where(d => d.UsuarioDestino.ID == usuarioOrigem.ID));
            Assert.Single(actual.Where(d => d.UsuarioDestino.ID == usuarioDestino.ID));
            Assert.True(actual.All(d => d.ConteudoMensagem == mensagemFormatada));
        }

        [Fact]
        public void ListarMensagemEntrarSala_ListaMensagens()
        {
            // Arrange
            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new List<Usuario> { usuario } };
            var conteudoMensagemUsuario = "Você entrou como 'Thiago' na sala #Geral";
            var conteudoMensagemTodos = "'Thiago' entrou na sala #Geral";
            var conteudoMensagemInstrucoesTermosLista = new [] { "## INSTRUÇÕES ##", "/ajuda", "/sair", "/p", "@Fulano", "ENTER" };
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ListarMensagemEntrarSala(usuario, sala);

            // Assert
            Assert.Equal(11, actual.ToList().Count);
            Assert.True(actual.All(d => d.IndicadorMensagemPrivada == false));
            Assert.Equal(10, actual.Count(d => d.UsuarioOrigem == null && d.UsuarioDestino.ID == usuario.ID));
            Assert.Single(actual.Where(d => d.UsuarioDestino == null && d.UsuarioOrigem.ID == usuario.ID));
            Assert.Single(actual.Where(d => d.ConteudoMensagem == conteudoMensagemUsuario));
            Assert.Single(actual.Where(d => d.ConteudoMensagem == conteudoMensagemTodos));
            Assert.True(conteudoMensagemInstrucoesTermosLista.All(d => actual.Any(a => a.ConteudoMensagem.Contains(d))));
        }

        [Fact]
        public void ListarMensagemInstrucoes_ListaMensagens()
        {
            // Arrange
            var usuario = new Usuario { Apelido = "Thiago" };
            var conteudoMensagemInstrucoesTermosLista = new[] { "## INSTRUÇÕES ##", "/ajuda", "/sair", "/p", "@Fulano", "ENTER" };
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ListarMensagemInstrucoes(usuario);

            // Assert
            Assert.Equal(9, actual.ToList().Count);
            Assert.True(actual.All(d => d.IndicadorMensagemPrivada == false));
            Assert.Equal(9, actual.Count(d => d.UsuarioOrigem == null && d.UsuarioDestino.ID == usuario.ID));
            Assert.True(conteudoMensagemInstrucoesTermosLista.All(d => actual.Any(a => a.ConteudoMensagem.Contains(d))));
        }

        [Fact]
        public void ListarMensagemSairSala_ListaMensagens()
        {
            // Arrange
            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new List<Usuario> { usuario } };
            var conteudoMensagemUsuario = "Você saiu da sala #Geral";
            var conteudoMensagemTodos = "'Thiago' saiu da sala #Geral";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ListarMensagemSairSala(usuario, sala);

            // Assert
            Assert.Equal(2, actual.ToList().Count);
            Assert.True(actual.All(d => d.IndicadorMensagemPrivada == false));
            Assert.Single(actual.Where(d => d.UsuarioOrigem == null && d.UsuarioDestino.ID == usuario.ID));
            Assert.Single(actual.Where(d => d.UsuarioDestino == null && d.UsuarioOrigem.ID == usuario.ID));
            Assert.Single(actual.Where(d => d.ConteudoMensagem == conteudoMensagemUsuario));
            Assert.Single(actual.Where(d => d.ConteudoMensagem == conteudoMensagemTodos));
        }


        [Fact]
        public void PossuiComando_MensagemSemComando_False()
        {
            // Arrange
            var mensagem = "Oi galera";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiComando(mensagem);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData("/ajuda")]
        [InlineData("/sair")]
        public void PossuiComando_MendagemComando_Verdadeiro(string mensagem)
        {
            // Arrange
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiComando(mensagem);

            // Assert
            Assert.True(actual);
        }


        [Fact]
        public void PossuiComandoSair_MensagemComandoSair_Verdadeiro()
        {
            // Arrange
            var mensagem = "/sair";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiComandoSair(mensagem);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("/ajuda")]
        [InlineData("Oi galera")]
        [InlineData("/p @Francine Oi, tudo bem")]
        [InlineData("@Francine Oi, tudo bem")]
        public void PossuiComandoSair_MensagemSemComandoSair_Falso(string mensagem)
        {
            // Arrange
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiComandoSair(mensagem);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void PossuiComandoAjuda_MensagemComandoAjuda_Verdadeiro()
        {
            // Arrange
            var mensagem = "/ajuda";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiComandoAjuda(mensagem);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("/sair")]
        [InlineData("Oi galera")]
        [InlineData("/p @Francine Oi, tudo bem")]
        [InlineData("@Francine Oi, tudo bem")]
        public void PossuiComandoAjuda_MensagemSemComandoAjuda_Falso(string mensagem)
        {
            // Arrange
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiComandoAjuda(mensagem);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData("/p @Thiago", "Thiago")]
        [InlineData("@Thiago", "Thiago")]
        public void ObterApelidoUsuarioDestino_MensagemUsuarioDestino_Apelido(string mensagem, string esperado)
        {
            // Arrange
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ObterApelidoUsuarioDestino(mensagem);

            // Assert
            Assert.Equal(esperado, actual);
        }

        [Fact]
        public void ObterApelidoUsuarioDestino_MensagemSemUsuarioDestino_Nulo()
        {
            // Arrange
            var mensagem = "Olá galera";
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.ObterApelidoUsuarioDestino(mensagem);

            // Assert
            Assert.True(actual == null);
        }


        [Theory]
        [InlineData("/p @Francine Oi, tudo bem")]
        [InlineData("@Francine Oi, tudo bem")]
        public void PossuiUsuarioDestino_MensagemUsuarioDestino_Verdadeito(string mensagem)
        {
            // Arrange
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiUsuarioDestino(mensagem);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("Oi, tudo bem")]
        [InlineData("Francine tudo bem")]
        public void PossuiUsuarioDestino_MensagemSemUsuarioDestino_Falso(string mensagem)
        {
            // Arrange
            var mensagemServico = ObterIMensagemServico();

            // Act
            var actual = mensagemServico.PossuiUsuarioDestino(mensagem);

            // Assert
            Assert.False(actual);
        }

    }
}
