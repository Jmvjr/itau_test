using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.ValueObjects;

public class QuantidadeTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000000)]
    public void Deve_CriarQuantidade_ComValorValido(int valor)
    {
        // Arrange & Act
        var qtd = new Quantidade(valor);

        // Assert
        qtd.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-1000)]
    public void Deve_Rejeitar_QuantidadeNegativa(int valor)
    {
        // Arrange & Act & Assert
        var act = () => new Quantidade(valor);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*não negativo*");
    }

    [Fact]
    public void Deve_Retornar_Lote_Quando_QuantidadeMaiorOuIgual100()
    {
        // Arrange
        var qtd = new Quantidade(150);

        // Act & Assert
        qtd.ObterQuantidadeLotesPadrao().Should().Be(1);
    }

    [Fact]
    public void Deve_Retornar_Fracionario_Quando_QuantidadeMenor100()
    {
        // Arrange
        var qtd = new Quantidade(50);

        // Act & Assert
        qtd.ObterQuantidadeFracionaria().Should().Be(50);
    }

    [Fact]
    public void Deve_Separar_Lotes_E_Fracionarios_Corretamente()
    {
        // Arrange
        var qtd = new Quantidade(250);

        // Act
        var lotes = qtd.ObterQuantidadeLotesPadrao();
        var fracionario = qtd.ObterQuantidadeFracionaria();

        // Assert
        (lotes * 100 + fracionario).Should().Be(250);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    [InlineData(1000)]
    public void Deve_Ter_Apenas_Lotes(int valor)
    {
        // Arrange
        var qtd = new Quantidade(valor);

        // Act & Assert
        qtd.ObterQuantidadeFracionaria().Should().Be(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    public void Deve_Ter_Apenas_Fracionarios(int valor)
    {
        // Arrange
        var qtd = new Quantidade(valor);

        // Act & Assert
        qtd.ObterQuantidadeLotesPadrao().Should().Be(0);
    }

    [Fact]
    public void Adicionar_DeveAumentarQuantidade()
    {
        // Arrange
        var qtd1 = new Quantidade(100);
        var qtd2 = new Quantidade(50);

        // Act
        var resultado = qtd1.Adicionar(qtd2);

        // Assert
        resultado.Valor.Should().Be(150);
    }

    [Fact]
    public void Subtrair_DeveAumentarQuantidade()
    {
        // Arrange
        var qtd1 = new Quantidade(100);
        var qtd2 = new Quantidade(30);

        // Act
        var resultado = qtd1.Subtrair(qtd2);

        // Assert
        resultado.Valor.Should().Be(70);
    }

    [Fact]
    public void Subtrair_Deve_Lancar_Excecao_SeResultadoNegativo()
    {
        // Arrange
        var qtd1 = new Quantidade(30);
        var qtd2 = new Quantidade(100);

        // Act & Assert
        var act = () => qtd1.Subtrair(qtd2);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*não pode ser negativa*");
    }

    [Fact]
    public void Deve_SerIgual_QuandoValoresSaoIguais()
    {
        // Arrange
        var qtd1 = new Quantidade(100);
        var qtd2 = new Quantidade(100);

        // Act & Assert
        qtd1.Should().Be(qtd2);
        (qtd1 == qtd2).Should().BeTrue();
    }

    [Fact]
    public void Deve_SerDiferente_QuandoValoresSaoDiferentes()
    {
        // Arrange
        var qtd1 = new Quantidade(100);
        var qtd2 = new Quantidade(200);

        // Act & Assert
        qtd1.Should().NotBe(qtd2);
        (qtd1 != qtd2).Should().BeTrue();
    }

    [Fact]
    public void Operador_Menor_DeveSerValido()
    {
        // Arrange
        var qtd1 = new Quantidade(100);
        var qtd2 = new Quantidade(200);

        // Act & Assert
        (qtd1 < qtd2).Should().BeTrue();
    }

    [Fact]
    public void Operador_Maior_DeveSerValido()
    {
        // Arrange
        var qtd1 = new Quantidade(200);
        var qtd2 = new Quantidade(100);

        // Act & Assert
        (qtd1 > qtd2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveRetornarValor()
    {
        // Arrange
        var qtd = new Quantidade(250);

        // Act & Assert
        qtd.ToString().Should().Contain("250");
    }
}
