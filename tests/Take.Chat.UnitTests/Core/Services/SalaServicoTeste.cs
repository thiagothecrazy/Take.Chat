using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using Take.Chat.Core;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;
using Take.Chat.Core.Services;
using Xunit;

namespace Take.Chat.UnitTests.Core.Services
{
    public class SalaServicoTeste
    {
        private ISalaServico ObterSalaServico()
        {
            return new SalaServico();
        }

        [Fact]
        public void CriarUsuario_NomeValido_ResultadoOk()
        {
            // Arrange
            var nome = "Amigos da Take";
            var servico = ObterSalaServico();

            // Act
            var actual = servico.CriarSala(nome);

            // Assert
            Assert.True(actual.Sucesso);
            Assert.Equal(nome, actual.Objeto.Nome);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CriarUsuario_NomeInvalido_ResultadoErro(string apelido)
        {
            // Arrange
            var mensagemErro = "Nome inválido.";
            var servico = ObterSalaServico();
            
            // Act
            var actual = servico.CriarSala(apelido);

            // Assert
            Assert.False(actual.Sucesso);
            Assert.Contains(actual.Notificacoes, d => d == mensagemErro);
        }

    }
}
