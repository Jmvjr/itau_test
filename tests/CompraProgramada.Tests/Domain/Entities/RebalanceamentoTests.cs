using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Enums;
using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class RebalanceamentoTests
{
    [Fact]
    public void Deve_Criar_Rebalanceamento_Por_Mudanca_Cesta()
    {
        // Arrange
        var tickerVendido = new Ticker("PETR4");
        var tickerComprado = new Ticker("VALE3");

        // Act
        var rebalanceamento = Rebalanceamento.CriarPorMudancaCesta(
            clienteId: 123,
            tickerVendido: tickerVendido,
            tickerComprado: tickerComprado,
            valorVenda: 5000m);

        // Assert
        rebalanceamento.ClienteId.Should().Be(123);
        rebalanceamento.Tipo.Should().Be(TipoRebalanceamento.MudancaCesta);
        rebalanceamento.TickerVendido.Valor.Should().Be("PETR4");
        rebalanceamento.TickerComprado.Valor.Should().Be("VALE3");
        rebalanceamento.ValorVenda.Should().Be(5000m);
    }

    [Fact]
    public void Deve_Criar_Rebalanceamento_Por_Desvio()
    {
        // Arrange
        var tickerVendido = new Ticker("PETR4");
        var tickerComprado = new Ticker("WEGE3");

        // Act
        var rebalanceamento = Rebalanceamento.CriarPorDesvio(
            clienteId: 456,
            tickerVendido: tickerVendido,
            tickerComprado: tickerComprado,
            valorVenda: 3000m);

        // Assert
        rebalanceamento.ClienteId.Should().Be(456);
        rebalanceamento.Tipo.Should().Be(TipoRebalanceamento.Desvio);
        rebalanceamento.TickerVendido.Valor.Should().Be("PETR4");
        rebalanceamento.TickerComprado.Valor.Should().Be("WEGE3");
        rebalanceamento.ValorVenda.Should().Be(3000m);
    }

    [Fact]
    public void Deve_Registrar_Data_Rebalanceamento()
    {
        // Arrange & Act
        var rebalanceamento = Rebalanceamento.CriarPorMudancaCesta(
            clienteId: 123,
            tickerVendido: new Ticker("PETR4"),
            tickerComprado: new Ticker("VALE3"),
            valorVenda: 5000m);

        // Assert
        rebalanceamento.DataRebalanceamento.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deve_Rejeitar_Valor_Venda_Negativo()
    {
        // Arrange & Act & Assert
        var act = () => Rebalanceamento.CriarPorMudancaCesta(
            clienteId: 123,
            tickerVendido: new Ticker("PETR4"),
            tickerComprado: new Ticker("VALE3"),
            valorVenda: -5000m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_Valor_Venda_Zero()
    {
        // Arrange & Act & Assert
        var act = () => Rebalanceamento.CriarPorMudancaCesta(
            clienteId: 123,
            tickerVendido: new Ticker("PETR4"),
            tickerComprado: new Ticker("VALE3"),
            valorVenda: 0m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_Ticker_Vendido_Nulo()
    {
        // Arrange & Act & Assert
        var act = () => Rebalanceamento.CriarPorMudancaCesta(
            clienteId: 123,
            tickerVendido: null,
            tickerComprado: new Ticker("VALE3"),
            valorVenda: 5000m);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Deve_Rejeitar_Ticker_Comprado_Nulo()
    {
        // Arrange & Act & Assert
        var act = () => Rebalanceamento.CriarPorMudancaCesta(
            clienteId: 123,
            tickerVendido: new Ticker("PETR4"),
            tickerComprado: null,
            valorVenda: 5000m);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToString_Deve_Retornar_Formato_Legivel()
    {
        // Arrange
        var rebalanceamento = Rebalanceamento.CriarPorMudancaCesta(
            clienteId: 123,
            tickerVendido: new Ticker("PETR4"),
            tickerComprado: new Ticker("VALE3"),
            valorVenda: 5000m);

        // Act
        var resultado = rebalanceamento.ToString();

        // Assert
        resultado.Should().Contain("MudancaCesta");
        resultado.Should().Contain("PETR4");
        resultado.Should().Contain("VALE3");
        resultado.Should().Contain("5000");
    }
}
