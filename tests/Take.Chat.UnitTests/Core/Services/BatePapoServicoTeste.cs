using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Take.Chat.Core;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;
using Take.Chat.Core.Services;
using Take.Chat.Infrastructure.Data;
using Xunit;

namespace Take.Chat.UnitTests.Core.Services
{
    public class BatePapoServicoTeste
    {
        #region Mock

        private ISalaServico ObterSalaServico(Sala objeto = null)
        {
            Resultado<Sala> resultado = objeto != null ? Resultado<Sala>.Ok(objeto) : Resultado<Sala>.Erro(new[] { "Erro Sala" });

            var mockServico = new Mock<ISalaServico>();
            mockServico.Setup(mc => mc.CriarSala(It.IsAny<string>())).Returns(resultado);

            return mockServico.Object;
        }

        private IUsuarioServico ObterUsuarioServico(Usuario objeto = null)
        {
            Resultado<Usuario> resultado = objeto != null ? Resultado<Usuario>.Ok(objeto) : Resultado<Usuario>.Erro(new[] { "Erro Usuario" });

            var mockServico = new Mock<IUsuarioServico>();
            mockServico.Setup(mc => mc.CriarUsuario(It.IsAny<string>())).Returns(resultado);

            return mockServico.Object;
        }

        private IMensagemServico ObterMensagemServico(bool possuiComando = false, bool possuiComandoAjuda = false, bool possuiComandoSair = false, bool possuiUsuarioDestino = false, string apelido = null)
        {
            var mensagens = new List<Mensagem>();
            mensagens.Add(new Mensagem { UsuarioOrigem = new Usuario { Apelido = "Thiago" }, UsuarioDestino = null, ConteudoMensagem = "Mensagem" });
            mensagens.Add(new Mensagem { UsuarioOrigem = new Usuario { Apelido = "Thiago" }, UsuarioDestino = new Usuario { Apelido = "Francine" }, ConteudoMensagem = "Mensagem" });

            var mockServico = new Mock<IMensagemServico>();
            mockServico.Setup(mc => mc.ListarMensagemEntrarSala(It.IsAny<Usuario>(), It.IsAny<Sala>())).Returns(mensagens);
            mockServico.Setup(mc => mc.ListarMensagemSairSala(It.IsAny<Usuario>(), It.IsAny<Sala>())).Returns(mensagens);
            mockServico.Setup(mc => mc.ListarMensagemInstrucoes(It.IsAny<Usuario>())).Returns(mensagens);
            mockServico.Setup(mc => mc.ListarMensagemParaTodos(It.IsAny<Usuario>(), It.IsAny<string>())).Returns(mensagens);
            mockServico.Setup(mc => mc.ListarMensagemParaUsuario(It.IsAny<Usuario>(), It.IsAny<Usuario>(), It.IsAny<string>())).Returns(mensagens);

            mockServico.Setup(mc => mc.PossuiComando(It.IsAny<string>())).Returns(possuiComando);
            mockServico.Setup(mc => mc.PossuiComandoAjuda(It.IsAny<string>())).Returns(possuiComandoAjuda);
            mockServico.Setup(mc => mc.PossuiComandoSair(It.IsAny<string>())).Returns(possuiComandoSair);
            mockServico.Setup(mc => mc.PossuiUsuarioDestino(It.IsAny<string>())).Returns(possuiUsuarioDestino);
            mockServico.Setup(mc => mc.ObterApelidoUsuarioDestino(It.IsAny<string>())).Returns(apelido);

            return mockServico.Object;
        }

        private IBatePapoRepositorio ObterBatePapoRepositorio(BatePapo batepapo = null, Sala sala = null, Usuario usuario = null)
        {
            var mockServico = new Mock<IBatePapoRepositorio>();

            mockServico.Setup(mc => mc.ObterBatePapo()).Returns(batepapo);

            mockServico.Setup(mc => mc.ObterSalaPeloNome(It.IsAny<string>())).Returns(sala);
            mockServico.Setup(mc => mc.ObterSalaPeloUsuario(It.IsAny<Usuario>())).Returns(sala);

            mockServico.Setup(mc => mc.ObterUsuarioPeloApelido(It.IsAny<string>())).Returns(usuario);
            mockServico.Setup(mc => mc.ObterUsuarioPeloID(It.IsAny<string>())).Returns(usuario);

            mockServico.Setup(mc => mc.AdicionarUsuarioSala(It.IsAny<Usuario>(), It.IsAny<Sala>())).Verifiable();
            mockServico.Setup(mc => mc.AdicionarSala(It.IsAny<Sala>())).Verifiable();

            return mockServico.Object;
        }

        private IBatePapoServico ObterBatePapoServico(ISalaServico salaServico, IUsuarioServico usuarioServico, IMensagemServico mensagemServico, IBatePapoRepositorio batePapoRepositorio)
        {
            return new BatePapoServico(salaServico, usuarioServico, batePapoRepositorio, mensagemServico);
        }

        #endregion

        [Fact]
        public void AdicionarUsuarioSala_NovaSalaNovoApelido_ResultadoOk()
        {
            // Arrange
            var apelido = "Thiago";
            var nomeSala = "Geral";

            var salaServico = ObterSalaServico(new Sala { Nome = "Geral" });
            var usuarioServico = ObterUsuarioServico(new Usuario { Apelido = "Thiago" });
            var mensagemServico = ObterMensagemServico();
            var batePapo = new BatePapo { Salas = new[] { new Sala { Nome = "Geral", Usuarios = new[] { new Usuario { Apelido = "Thiago" } } } } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

            // Assert
            Assert.True(actual.Sucesso);
            Assert.Single(actual.Objeto.Salas);
            Assert.Single(actual.Objeto.Salas.First().Usuarios);
            Assert.Equal(nomeSala, actual.Objeto.Salas.First().Nome);
            Assert.Equal(apelido, actual.Objeto.Salas.First().Usuarios.First().Apelido);
        }

        [Fact]
        public void AdicionarUsuarioSala_NovoApelidoInvalido_ResultadoErro()
        {
            // Arrange
            var apelido = " ";
            var nomeSala = "Geral";

            var salaServico = ObterSalaServico(new Sala { Nome = "Geral" });
            var usuarioServico = ObterUsuarioServico();
            var mensagemServico = ObterMensagemServico();
            var batePapoRepositorio = ObterBatePapoRepositorio();
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

            // Assert
            Assert.False(actual.Sucesso);
            Assert.True(actual.Notificacoes.Any());
        }

        [Fact]
        public void AdicionarUsuarioSala_NovaSalaInvalido_ResultadoErro()
        {
            // Arrange
            var apelido = "Thiago";
            var nomeSala = " ";

            var salaServico = ObterSalaServico();
            var usuarioServico = ObterUsuarioServico(new Usuario { Apelido = "Thiago" });
            var mensagemServico = ObterMensagemServico();
            var batePapoRepositorio = ObterBatePapoRepositorio();
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

            // Assert
            Assert.False(actual.Sucesso);
            Assert.True(actual.Notificacoes.Any());
        }

        [Fact]
        public void AdicionarUsuarioSala_SalaExistente_ResultadoOK()
        {
            // Arrange
            var apelido = "Thiago";
            var nomeSala = "Geral";

            var sala = new Sala { Nome = "Geral" };
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(new Usuario { Apelido = "Thiago" });
            var mensagemServico = ObterMensagemServico();
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, null);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

            // Assert
            Assert.True(actual.Sucesso);
            Assert.Contains(actual.Objeto.Salas, d => d.Nome == nomeSala);
        }

        [Fact]
        public void AdicionarUsuarioSala_ApelidoExistente_ResultadoErro()
        {
            // Arrange
            var apelido = "Thiago";
            var nomeSala = "Geral";

            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario } };
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var mensagemServico = ObterMensagemServico();
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, usuario);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

            // Assert
            Assert.False(actual.Sucesso);
            Assert.True(actual.Notificacoes.Any());
            Assert.Contains(actual.Notificacoes, d => d == "Apelido já esta sendo usado.");
        }

        //-------------------------------

        [Fact]
        public void ObterUsuario_ApelidoExistente_Usuario()
        {
            // Arrange
            var apelido = "Thiago";

            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario } };
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var mensagemServico = ObterMensagemServico();
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, usuario);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.ObterUsuario(apelido);

            // Assert
            Assert.Equal(apelido, actual.Apelido);


        }

        //-------------------------------

        [Fact]
        public void EntrarSala_UsuarioID_Verdadeiro()
        {
            // Arrange
            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario } };
            
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var mensagemServico = ObterMensagemServico();
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, usuario);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.EntrarSala(usuario.ID.ToString());

            // Assert
            Assert.True(actual);


        }

        //-------------------------------


        [Fact]
        public void ProcessarMensagem_ComandoAjuda_Verdadeiro()
        {
            // Arrange
            string mensagem = "/ajuda";
            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario } };

            var mensagemServico = ObterMensagemServico(true, true, false, false, null);
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, usuario);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.ProcessarMensagem(usuario.ID.ToString(), mensagem);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void ProcessarMensagem_ComandoSair_Verdadeiro()
        {
            // Arrange
            string mensagem = "/sair";
            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario } };

            var mensagemServico = ObterMensagemServico(true, false, true, false, null);
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, usuario);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.ProcessarMensagem(usuario.ID.ToString(), mensagem);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void ProcessarMensagem_MensagemParaTodos_Verdadeiro()
        {
            // Arrange
            string mensagem = "Olá galera";
            var usuario = new Usuario { Apelido = "Thiago" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario } };

            var mensagemServico = ObterMensagemServico(false, false, false, false, null);
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, usuario);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.ProcessarMensagem(usuario.ID.ToString(), mensagem);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void ProcessarMensagem_MensagemParaUsuario_Verdadeiro()
        {
            // Arrange
            string mensagem = "@Francine Oi tudo bem?";
            var usuario = new Usuario { Apelido = "Thiago" };
            var outroUsuario = new Usuario { Apelido = "Francine" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario, outroUsuario } };

            var mensagemServico = ObterMensagemServico(false, false, false, true, "Francine");
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, usuario);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.ProcessarMensagem(usuario.ID.ToString(), mensagem);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void ProcessarMensagem_MensagemParaUsuarioInexistente_Verdadeiro()
        {
            // Arrange
            string mensagem = "@Usuario Oi tudo bem?";
            var usuario = new Usuario { Apelido = "Thiago" };
            var outroUsuario = new Usuario { Apelido = "Francine" };
            var sala = new Sala { Nome = "Geral", Usuarios = new[] { usuario, outroUsuario } };

            var mensagemServico = ObterMensagemServico(false, false, false, true, "Usuario");
            var salaServico = ObterSalaServico(sala);
            var usuarioServico = ObterUsuarioServico(usuario);
            var batePapo = new BatePapo { Salas = new[] { sala } };
            var batePapoRepositorio = ObterBatePapoRepositorio(batePapo, sala, null);
            var batePapoServico = ObterBatePapoServico(salaServico, usuarioServico, mensagemServico, batePapoRepositorio);

            // Act
            var actual = batePapoServico.ProcessarMensagem(usuario.ID.ToString(), mensagem);

            // Assert
            Assert.True(actual);
        }

    }
}
