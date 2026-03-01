using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class CustodiaTests
{
    [Fact]
    public void Deve_Criar_Custodia_Com_Dados_Validos()
    {
        // Arrange & Act
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.50m);

        // Assert
        custodia.ContaGraficaId.Should().Be(1);
        custodia.Ticker.Valor.Should().Be("PETR4");
        custodia.Quantidade.Valor.Should().Be(100);
        custodia.PrecoMedio.Should().Be(20.50m);
    }

    [Fact]
    public void Deve_Calcular_Valor_Investido()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.50m);

        // Act
        var valorInvestido = custodia.CalcularValorInvestido();

        // Assert
        valorInvestido.Should().Be(2050m); // 100 * 20.50
    }

    [Fact]
    public void Deve_Calcular_Valor_Atual()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.50m);
        var cotacaoAtual = 25.00m;

        // Act
        var valorAtual = custodia.CalcularValorAtual(cotacaoAtual);

        // Assert
        valorAtual.Should().Be(2500m); // 100 * 25.00
    }

    [Fact]
    public void Deve_Calcular_P_L_Positivo()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.50m);
        var cotacaoAtual = 25.00m;

        // Act
        var pl = custodia.CalcularPL(cotacaoAtual);

        // Assert
        pl.Should().Be(450m); // (25.00 - 20.50) * 100
    }

    [Fact]
    public void Deve_Calcular_P_L_Negativo()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 25.50m);
        var cotacaoAtual = 20.00m;

        // Act
        var pl = custodia.CalcularPL(cotacaoAtual);

        // Assert
        pl.Should().Be(-550m); // (20.00 - 25.50) * 100
    }

    [Fact]
    public void Deve_Atualizar_Com_Nova_Compra_E_Recalcular_PM()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.00m);
        var qtdAdicionada = new Quantidade(100);
        var precoNovaCompra = 25.00m;

        // Act
        custodia.AtualizarComNovaCompra(qtdAdicionada, precoNovaCompra);

        // Assert
        custodia.Quantidade.Valor.Should().Be(200);
        // PM = (100 * 20 + 100 * 25) / 200 = 22.5
        custodia.PrecoMedio.Should().Be(22.5m);
    }

    [Fact]
    public void Deve_Vender_Sem_Alterar_Preco_Medio()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.00m);
        var precoMedioOriginal = custodia.PrecoMedio;

        // Act
        custodia.Vender(new Quantidade(30));

        // Assert
        custodia.Quantidade.Valor.Should().Be(70);
        custodia.PrecoMedio.Should().Be(precoMedioOriginal); // PM não muda
    }

    [Fact]
    public void Deve_Rejeitar_Venda_Acima_Da_Quantidade_Disponivel()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.00m);

        // Act & Assert
        var act = () => custodia.Vender(new Quantidade(150));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_Venda_De_Quantidade_Negativa()
    {
        // Arrange
        var ticker = new Ticker("PETR4");
        var quantidade = new Quantidade(100);
        var custodia = new Custodia(1, ticker, quantidade, 20.00m);

        // Act & Assert
        var act = () => custodia.Vender(new Quantidade(-10));
        act.Should().Throw<ArgumentException>();
    }
}
