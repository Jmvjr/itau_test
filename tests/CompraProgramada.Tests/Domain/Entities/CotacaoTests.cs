using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class CotacaoTests
{
    [Fact]
    public void Deve_Criar_Cotacao_Com_Dados_Validos()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var data = DateTime.UtcNow.Date;

        // Act
        var cotacao = new Cotacao(data, ticker, precoAbertura: 25.00m, precoFechamento: 25.75m, precoMaximo: 26.50m, precoMinimo: 24.50m);

        // Assert
        cotacao.Ticker.Valor.Should().Be("PETR4");
        cotacao.DataPregao.Should().Be(data);
        cotacao.PrecoAbertura.Should().Be(25.00m);
        cotacao.PrecoMaximo.Should().Be(26.50m);
        cotacao.PrecoMinimo.Should().Be(24.50m);
        cotacao.PrecoFechamento.Should().Be(25.75m);
    }

    [Fact]
    public void Deve_Validar_Maximo_Maior_Ou_Igual_Minimo()
    {
        // Arrange & Act & Assert
        var ticker = new Ticker("PETR4");
        var data = DateTime.UtcNow.Date;

        var act = () => new Cotacao(data, ticker, precoAbertura: 25.00m, precoFechamento: 24.00m, precoMaximo: 23.00m, precoMinimo: 25.00m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Validar_Abertura_Entre_Maximo_E_Minimo()
    {
        // Arrange & Act & Assert
        var ticker = new Ticker("PETR4");
        var data = DateTime.UtcNow.Date;

        // Abertura 27 > Máximo 26
        var act = () => new Cotacao(data, ticker, precoAbertura: 27.00m, precoFechamento: 25.00m, precoMaximo: 26.50m, precoMinimo: 24.50m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Validar_Fechamento_Entre_Maximo_E_Minimo()
    {
        // Arrange & Act & Assert
        var ticker = new Ticker("PETR4");
        var data = DateTime.UtcNow.Date;

        // Fechamento 23 < Mínimo 24.50
        var act = () => new Cotacao(data, ticker, precoAbertura: 25.00m, precoFechamento: 23.00m, precoMaximo: 26.50m, precoMinimo: 24.50m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Aceitar_Valores_OHLC_Iguais()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var data = DateTime.UtcNow.Date;

        // Act - Todos os preços iguais (dia sem movimentação)
        var cotacao = new Cotacao(data, ticker, precoAbertura: 25.00m, precoFechamento: 25.00m, precoMaximo: 25.00m, precoMinimo: 25.00m);

        // Assert
        cotacao.PrecoAbertura.Should().Be(25.00m);
        cotacao.PrecoMaximo.Should().Be(25.00m);
        cotacao.PrecoMinimo.Should().Be(25.00m);
        cotacao.PrecoFechamento.Should().Be(25.00m);
    }

    [Fact]
    public void Deve_Armazenar_Precision_Monetaria_Decimal()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var data = DateTime.UtcNow.Date;

        // Act
        var cotacao = new Cotacao(data, ticker, precoAbertura: 25.156m, precoFechamento: 25.789m, precoMaximo: 25.987m, precoMinimo: 24.001m);

        // Assert
        cotacao.PrecoAbertura.Should().Be(25.156m);
        cotacao.PrecoMaximo.Should().Be(25.987m);
        cotacao.PrecoMinimo.Should().Be(24.001m);
        cotacao.PrecoFechamento.Should().Be(25.789m);
    }
}
