using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.ValueObjects;

public class TickerTests
{
    [Theory]
    [InlineData("PETR4")]
    [InlineData("VALE3")]
    [InlineData("ITUB4")]
    [InlineData("PETR4F")]
    [InlineData("B")]
    [InlineData("ABCDEFGHIJ")]
    public void Deve_CriarTicker_ComValorValido(string ticker)
    {
        // Arrange & Act
        var result = new Ticker(ticker);

        // Assert
        result.Valor.Should().Be(ticker.ToUpper());
    }

    [Fact]
    public void Deve_Converter_ParaMaiuscula()
    {
        // Arrange & Act
        var ticker = new Ticker("petr4");

        // Assert
        ticker.Valor.Should().Be("PETR4");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_Rejeitar_TickerVazio(string tickerVazio)
    {
        // Arrange & Act & Assert
        var act = () => new Ticker(tickerVazio);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_TickerMaiorQue10Caracteres()
    {
        // Arrange & Act & Assert
        var act = () => new Ticker("ABCDEFGHIJK");
        act.Should().Throw<ArgumentException>()
            .WithMessage("*10*");
    }

    [Theory]
    [InlineData("PETR4@")]
    [InlineData("VALE#3")]
    [InlineData("ITUB$4")]
    public void Deve_Rejeitar_TickerComCaracteresEspeciais(string tickerInvalido)
    {
        // Arrange & Act & Assert
        var act = () => new Ticker(tickerInvalido);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*letras e números*");
    }

    [Fact]
    public void EhFracionario_Deve_RetornarTrue_QuandoTerminaComF()
    {
        // Arrange
        var ticker = new Ticker("PETR4F");

        // Act & Assert
        ticker.EhFracionario.Should().BeTrue();
    }

    [Fact]
    public void EhFracionario_Deve_RetornarFalse_QuandoNaoTerminaComF()
    {
        // Arrange
        var ticker = new Ticker("PETR4");

        // Act & Assert
        ticker.EhFracionario.Should().BeFalse();
    }

    [Fact]
    public void ObterTickerLotePadrao_DeveRemoverF()
    {
        // Arrange
        var ticker = new Ticker("PETR4F");

        // Act
        var resultado = ticker.ObterTikerLotePadrao();

        // Assert
        resultado.Valor.Should().Be("PETR4");
    }

    [Fact]
    public void ObterTickerLotePadrao_DeveRetornarMesmoTicker_SeNaoEhFracionario()
    {
        // Arrange
        var ticker = new Ticker("PETR4");

        // Act
        var resultado = ticker.ObterTikerLotePadrao();

        // Assert
        resultado.Valor.Should().Be("PETR4");
    }

    [Fact]
    public void Deve_SerIgual_QuandoValoresSaoIguais()
    {
        // Arrange
        var ticker1 = new Ticker("PETR4");
        var ticker2 = new Ticker("PETR4");

        // Act & Assert
        ticker1.Should().Be(ticker2);
        (ticker1 == ticker2).Should().BeTrue();
    }

    [Fact]
    public void Deve_SerDiferente_QuandoValoresSaoDiferentes()
    {
        // Arrange
        var ticker1 = new Ticker("PETR4");
        var ticker2 = new Ticker("VALE3");

        // Act & Assert
        ticker1.Should().NotBe(ticker2);
        (ticker1 != ticker2).Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveRetornarValor()
    {
        // Arrange
        var ticker = new Ticker("PETR4");

        // Act & Assert
        ticker.ToString().Should().Be("PETR4");
    }
}
