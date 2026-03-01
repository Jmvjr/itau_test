using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class CestaRecomendacaoTests
{
    [Fact]
    public void Deve_Criar_Cesta_Com_5_Ativos_E_100_Porcento()
    {
        // Arrange
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 10m)
        };

        // Act
        var cesta = new CestaRecomendacao("Top 5", itens);

        // Assert
        cesta.Nome.Should().Be("Top 5");
        cesta.Ativa.Should().BeTrue();
        cesta.Itens.Should().HaveCount(5);
    }

    [Fact]
    public void Deve_Rejeitar_Menos_De_5_Ativos()
    {
        // Arrange
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 40m),
            ("VALE3", 40m),
            ("ITUB4", 20m)
        };

        // Act & Assert
        var act = () => new CestaRecomendacao("Top 3", itens);
        act.Should().Throw<QuantidadeAtivosInvalidaException>();
    }

    [Fact]
    public void Deve_Rejeitar_Mais_De_5_Ativos()
    {
        // Arrange
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 20m),
            ("VALE3", 20m),
            ("ITUB4", 20m),
            ("WEGE3", 20m),
            ("B3SA3", 10m),
            ("BBAS3", 10m)
        };

        // Act & Assert
        var act = () => new CestaRecomendacao("Top 6", itens);
        act.Should().Throw<QuantidadeAtivosInvalidaException>();
    }

    [Fact]
    public void Deve_Rejeitar_Soma_Percentual_Diferente_De_100()
    {
        // Arrange - soma = 99%
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 9m)
        };

        // Act & Assert
        var act = () => new CestaRecomendacao("Soma Errada", itens);
        act.Should().Throw<PercentuaisInvalidosException>();
    }

    [Fact]
    public void Deve_Rejeitar_Soma_Percentual_Acima_De_100()
    {
        // Arrange - soma = 101%
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 11m)
        };

        // Act & Assert
        var act = () => new CestaRecomendacao("Soma Acima", itens);
        act.Should().Throw<PercentuaisInvalidosException>();
    }

    [Fact]
    public void Deve_Desativar_Cesta()
    {
        // Arrange
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 10m)
        };
        var cesta = new CestaRecomendacao("Top 5", itens);

        // Act
        cesta.Desativar();

        // Assert
        cesta.Ativa.Should().BeFalse();
    }

    [Fact]
    public void Deve_Obter_Percentual_Do_Ticker()
    {
        // Arrange
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 10m)
        };
        var cesta = new CestaRecomendacao("Top 5", itens);

        // Act
        var percentualPetr = cesta.ObterPercentualTicker(new Ticker("PETR4"));
        var percentualVale = cesta.ObterPercentualTicker(new Ticker("VALE3"));

        // Assert
        percentualPetr.Valor.Should().Be(30m);
        percentualVale.Valor.Should().Be(25m);
    }

    [Fact]
    public void Deve_Retornar_Zero_Para_Ticker_Nao_Existente()
    {
        // Arrange
        var itens = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 10m)
        };
        var cesta = new CestaRecomendacao("Top 5", itens);

        // Act
        var percentualInexistente = cesta.ObterPercentualTicker(new Ticker("BBAS3"));

        // Assert
        percentualInexistente.Valor.Should().Be(0m);
    }

    [Fact]
    public void Deve_Identificar_Tickers_Entrando()
    {
        // Arrange
        var cestaAntiga = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 10m)
        };
        var cestaNovatmp = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("BBAS3", 10m) // Novo ticker
        };

        var cestaAntigaObj = new CestaRecomendacao("Antiga", cestaAntiga);
        var cestaNova = new CestaRecomendacao("Nova", cestaNovatmp);

        // Act
        var tickersEntrando = cestaNova.IdentificarTickersEntrados(cestaAntigaObj);

        // Assert
        tickersEntrando.Should().Contain(new Ticker("BBAS3"));
    }

    [Fact]
    public void Deve_Identificar_Tickers_Saindo()
    {
        // Arrange
        var cestaAntiga = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("B3SA3", 10m)
        };
        var cestaNova = new List<(string ticker, decimal percentual)>
        {
            ("PETR4", 30m),
            ("VALE3", 25m),
            ("ITUB4", 20m),
            ("WEGE3", 15m),
            ("BBAS3", 10m)
        };

        var cestaAntigaObj = new CestaRecomendacao("Antiga", cestaAntiga);
        var cestaNovaObj = new CestaRecomendacao("Nova", cestaNova);

        // Act
        var tickersSaindo = cestaAntigaObj.IdentificarTickersSaidos(cestaNovaObj);

        // Assert
        tickersSaindo.Should().Contain(new Ticker("B3SA3"));
    }
}
