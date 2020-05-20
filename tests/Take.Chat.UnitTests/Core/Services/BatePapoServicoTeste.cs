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
    public class BatePapoServicoTeste
    {
        //private IBatePapoServico ObterBatePapoServico(BatePapo batePapo = null)
        //{
        //    var salaServico = new SalaServico();
        //    var usuarioServico = new UsuarioServico();
        //    var batePapoRepositorio = new BatePapoRepositorio();
        //    return new BatePapoServico(salaServico, usuarioServico, batePapo ?? new BatePapo());
        //}

        //[Fact]
        //public void AdicionarUsuarioSala_NovaSalaNovoApelido_ResultadoOk()
        //{
        //    // Arrange
        //    var apelido = "Thiago";
        //    var nomeSala = "Geral";
        //    var batePapoServico = ObterBatePapoServico();

        //    // Act
        //    var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

        //    // Assert
        //    Assert.True(actual.Sucesso);
        //    Assert.Single(actual.Objeto.Salas);
        //    Assert.Single(actual.Objeto.Salas.First().Value.Usuarios);
        //    Assert.Equal(nomeSala, actual.Objeto.Salas.First().Value.Nome);
        //    Assert.Equal(apelido, actual.Objeto.Salas.First().Value.Usuarios.First().Apelido);
        //}

        //[Fact]
        //public void AdicionarUsuarioSala_NovoApelidoInvalido_ResultadoErro()
        //{
        //    // Arrange
        //    var apelido = " ";
        //    var nomeSala = "Geral";
        //    var batePapoServico = ObterBatePapoServico();

        //    // Act
        //    var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

        //    // Assert
        //    Assert.False(actual.Sucesso);
        //    Assert.True(actual.Notificacoes.Any());
        //}

        //[Fact]
        //public void AdicionarUsuarioSala_NovaSalaInvalido_ResultadoErro()
        //{
        //    // Arrange
        //    var apelido = "Thiago";
        //    var nomeSala = " ";
        //    var batePapoServico = ObterBatePapoServico();

        //    // Act
        //    var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

        //    // Assert
        //    Assert.False(actual.Sucesso);
        //    Assert.True(actual.Notificacoes.Any());
        //}

        //[Fact]
        //public void AdicionarUsuarioSala_SalaExistente_ResultadoOK()
        //{
        //    // Arrange
        //    var apelido = "Thiago";
        //    var nomeSala = "Geral";

        //    var batePapo = new BatePapo();
        //    var sala = new Sala { Nome = nomeSala };
        //    batePapo.Salas.TryAdd(sala.ID.ToString(), sala);
        //    var batePapoServico = ObterBatePapoServico(batePapo);

        //    // Act
        //    var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

        //    // Assert
        //    Assert.True(actual.Sucesso);
        //    Assert.Contains(actual.Objeto.Salas, d => d.Value.Nome == nomeSala);
        //}

        //[Fact]
        //public void AdicionarUsuarioSala_ApelidoExistente_ResultadoErro()
        //{
        //    // Arrange
        //    var apelido = "Thiago";
        //    var nomeSala = "Geral";

        //    var batePapo = new BatePapo();
        //    var sala = new Sala { Nome = nomeSala };
        //    var usuario = new Usuario { Apelido = apelido };
        //    sala.Usuarios.Add(usuario);
        //    batePapo.Salas.TryAdd(sala.ID.ToString(), sala);
        //    var batePapoServico = ObterBatePapoServico(batePapo);

        //    // Act
        //    var actual = batePapoServico.AdicionarUsuarioSala(apelido, nomeSala);

        //    // Assert
        //    Assert.False(actual.Sucesso);
        //    Assert.True(actual.Notificacoes.Any());
        //}

    }
}
