using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using Take.Chat.Core;
using Take.Chat.Core.Entities;
using Take.Chat.Core.Interfaces;
using Take.Chat.Core.Services;
using Xunit;

namespace Take.Chat.UnitTests.Core.Services
{
    public class UsuarioServicoTeste
    {
        private IUsuarioServico ObterUsuarioServico()
        {
            return new UsuarioServico();
        }

        [Fact]
        public void CriarUsuario_ApelidoValido_ResultadoOk()
        {
            // Arrange
            var apelido = "Thiago";
            var usuarioServico = ObterUsuarioServico();

            // Act
            var actual = usuarioServico.CriarUsuario(apelido);

            // Assert
            Assert.True(actual.Sucesso);
            Assert.Equal(apelido, actual.Objeto.Apelido);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CriarUsuario_ApelidoInvalido_ResultadoErro(string apelido)
        {
            // Arrange
            var mensagemErro = "Apelido de usuário inválido.";
            var usuarioServico = ObterUsuarioServico();
            
            // Act
            var actual = usuarioServico.CriarUsuario(apelido);

            // Assert
            Assert.False(actual.Sucesso);
            Assert.Contains(actual.Notificacoes, d => d == mensagemErro);
        }

    }
}
