using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.ValueObjects;

public class CPFTests
{
    [Fact]
    public void Deve_CriarCPF_ComValorValido()
    {
        // Arrange & Act
        var cpf = new CPF("123.456.789-09");

        // Assert
        cpf.Valor.Should().Be("12345678909");
    }

    [Fact]
    public void Deve_RemoverFormatacao_Automaticamente()
    {
        // Arrange & Act
        var cpf1 = new CPF("123.456.789-09");
        var cpf2 = new CPF("12345678909");

        // Assert
        cpf1.Valor.Should().Be(cpf2.Valor);
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("222.222.222-22")]
    [InlineData("333.333.333-33")]
    [InlineData("000.000.000-00")]
    public void Deve_Rejeitar_SequenciasRepetidas(string cpfInvalido)
    {
        // Arrange & Act & Assert
        var act = () => new CPF(cpfInvalido);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*inválido*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_Rejeitar_ValorVazio(string cpfVazio)
    {
        // Arrange & Act & Assert
        var act = () => new CPF(cpfVazio);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_CPF_ComTamanhoInvalido()
    {
        // Arrange & Act & Assert
        var act = () => new CPF("123.456.789");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Ser_Igual_QuandoValoresSaoIguais()
    {
        // Arrange
        var cpf1 = new CPF("123.456.789-09");
        var cpf2 = new CPF("123.456.789-09");

        // Act & Assert
        cpf1.Should().Be(cpf2);
        cpf1.Equals(cpf2).Should().BeTrue();
        (cpf1 == cpf2).Should().BeTrue();
    }

    [Fact]
    public void Deve_Ser_Diferente_QuandoValoresSaoDiferentes()
    {
        // Arrange
        var cpf1 = new CPF("123.456.789-09");
        var cpf2 = new CPF("987.654.321-00");

        // Act & Assert
        cpf1.Should().NotBe(cpf2);
        cpf1.Equals(cpf2).Should().BeFalse();
        (cpf1 != cpf2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveRetornarValorFormatado()
    {
        // Arrange
        var cpf = new CPF("12345678909");

        // Act
        var resultado = cpf.ToString();

        // Assert
        resultado.Should().Be("123.456.789-09");
    }

    [Fact]
    public void GetHashCode_DeveSerIgual_ParaValoresIguais()
    {
        // Arrange
        var cpf1 = new CPF("123.456.789-09");
        var cpf2 = new CPF("123.456.789-09");

        // Act & Assert
        cpf1.GetHashCode().Should().Be(cpf2.GetHashCode());
    }
}
