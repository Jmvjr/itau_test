using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Enums;
using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class OrdemCompraTests
{
    [Fact]
    public void Deve_Criar_Ordem_Compra_Com_Dados_Validos()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);

        // Act
        var ordem = new OrdemCompra(
            contaMasterId: 1,
            ticker: ticker,
            quantidade: quantidade,
            precoUnitario: 25.50m,
            tipoMercado: TipoMercado.LotePadrao);

        // Assert
        ordem.ContaMasterId.Should().Be(1);
        ordem.Ticker.Valor.Should().Be("PETR4");
        ordem.Quantidade.Valor.Should().Be(100);
        ordem.PrecoUnitario.Should().Be(25.50m);
        ordem.TipoMercado.Should().Be(TipoMercado.LotePadrao);
    }

    [Fact]
    public void Deve_Calcular_Valor_Total()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var ordem = new OrdemCompra(1, ticker, quantidade, 25.50m, TipoMercado.LotePadrao);

        // Act
        var valorTotal = ordem.CalcularValorTotal();

        // Assert
        valorTotal.Should().Be(2550m); // 100 * 25.50
    }

    [Theory]
    [InlineData(TipoMercado.LotePadrao)]
    [InlineData(TipoMercado.Fracionario)]
    public void Deve_Aceitar_Ambos_Tipos_De_Mercado(TipoMercado tipo)
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);

        // Act
        var ordem = new OrdemCompra(1, ticker, quantidade, 25.50m, tipo);

        // Assert
        ordem.TipoMercado.Should().Be(tipo);
    }

    [Fact]
    public void Deve_Rejeitar_Preco_Unitario_Negativo()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);

        // Act & Assert
        var act = () => new OrdemCompra(1, ticker, quantidade, -25.50m, TipoMercado.LotePadrao);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_Preco_Unitario_Zero()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);

        // Act & Assert
        var act = () => new OrdemCompra(1, ticker, quantidade, 0m, TipoMercado.LotePadrao);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Aceitar_Precisao_Decimal_Ate_2_Casas()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);

        // Act
        var ordem = new OrdemCompra(1, ticker, quantidade, 25.99m, TipoMercado.LotePadrao);

        // Assert
        ordem.PrecoUnitario.Should().Be(25.99m);
    }
}
