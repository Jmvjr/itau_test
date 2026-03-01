using CompraProgramada.Application.Services;
using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.ValueObjects;
using Moq;
using Xunit;
using MediatR;

namespace CompraProgramada.Tests.Application.Services;

public class RebalanceamentoServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICotacaoService> _cotacaoServiceMock;
    private readonly Mock<IKafkaProducerService> _kafkaProducerMock;
    private readonly Mock<ICustodiaRepository> _custodiaRepositoryMock;
    private readonly Mock<IPublisher> _publisherMock;
    private readonly IRebalanceamentoService _service;

    public RebalanceamentoServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cotacaoServiceMock = new Mock<ICotacaoService>();
        _kafkaProducerMock = new Mock<IKafkaProducerService>();
        _custodiaRepositoryMock = new Mock<ICustodiaRepository>();
        _publisherMock = new Mock<IPublisher>();

        _service = new RebalanceamentoService(
            _unitOfWorkMock.Object,
            _cotacaoServiceMock.Object,
            _kafkaProducerMock.Object,
            _custodiaRepositoryMock.Object,
            _publisherMock.Object);
    }

    [Fact]
    public async Task ProcessarRebalanceamentoAsync_SemClientesAtivos_RetornaZero()
    {
        // Arrange
        var cestaAnterior = new CestaRecomendacao("Cesta A", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        // Mesma cesta - sem alterações detectadas
        var novaCesta = new CestaRecomendacao("Cesta A", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        _unitOfWorkMock
            .Setup(u => u.ClienteRepository.ObterTodosAsync())
            .ReturnsAsync(new List<Cliente>());

        // Act
        var resultado = await _service.ProcessarRebalanceamentoAsync(cestaAnterior, novaCesta);

        // Assert
        Assert.Equal(0, resultado);
    }

    [Fact]
    public async Task CalcularAjustesAsync_SemAlteracao_RetornaDetalhesVazio()
    {
        // Arrange
        var cliente = new Cliente("Cliente Teste", "12345678901", "teste@teste.com", 3000);

        // Mesma cesta - sem alterações
        var cestaAnterior = new CestaRecomendacao("Top5 Jan", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        var novaCesta = new CestaRecomendacao("Top5 Jan", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        _custodiaRepositoryMock
            .Setup(c => c.ObterTodosPorClienteAsync(cliente.Id))
            .ReturnsAsync(new List<Custodia>());

        _cotacaoServiceMock
            .Setup(c => c.ObterCotacaoPorDataAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new Cotacao(DateTime.UtcNow, new Ticker("PETR4"), 35, 35, 36, 34));

        // Act
        var detalhes = await _service.CalcularAjustesAsync(cliente, cestaAnterior, novaCesta);

        // Assert
        Assert.NotNull(detalhes);
        Assert.False(detalhes.TemAlteracoes);
        Assert.Equal(0, detalhes.TickersParaVender.Count);
        Assert.Equal(0, detalhes.TickersParaComprar.Count);
        Assert.Equal(0, detalhes.TickersParaRebalancear.Count);
    }

    [Fact]
    public async Task CalcularAjustesAsync_IdentificaTickersSaidos()
    {
        // Arrange
        var cliente = new Cliente("Cliente Teste", "12345678901", "teste@teste.com", 3000);

        var cestaAnterior = new CestaRecomendacao("Top5 Jan", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        var novaCesta = new CestaRecomendacao("Top5 Fev", new List<(string, decimal)>
        {
            ("PETR4", 35), ("VALE3", 30), ("ITUB4", 20), ("ABEV3", 10), ("RENT3", 5)
            // Saíram: BBDC4, WEGE3
            // Entraram: ABEV3, RENT3
        });

        // Cliente tem posição em BBDC4 e WEGE3 (que saíram)
        var custodia1 = new Custodia(cliente.Id, new Ticker("BBDC4"), new Quantidade(50), 15.00m);
        var custodia2 = new Custodia(cliente.Id, new Ticker("WEGE3"), new Quantidade(30), 40.00m);

        _custodiaRepositoryMock
            .Setup(c => c.ObterTodosPorClienteAsync(cliente.Id))
            .ReturnsAsync(new List<Custodia> { custodia1, custodia2 });

        _cotacaoServiceMock
            .Setup(c => c.ObterCotacaoPorDataAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new Cotacao(DateTime.UtcNow, new Ticker("PETR4"), 35, 35, 36, 34));

        // Act
        var detalhes = await _service.CalcularAjustesAsync(cliente, cestaAnterior, novaCesta);

        // Assert
        Assert.NotNull(detalhes);
        Assert.Equal(2, detalhes.TickersParaVender.Count);
        Assert.Contains("BBDC4", detalhes.TickersParaVender.Keys.Select(k => k.Valor));
        Assert.Contains("WEGE3", detalhes.TickersParaVender.Keys.Select(k => k.Valor));
        Assert.True(detalhes.TemAlteracoes);
    }

    [Fact]
    public async Task CalcularAjustesAsync_IdentificaTickersEntrados()
    {
        // Arrange
        var cliente = new Cliente("Cliente Teste", "12345678901", "teste@teste.com", 3000);

        var cestaAnterior = new CestaRecomendacao("Top5 Jan", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        var novaCesta = new CestaRecomendacao("Top5 Fev", new List<(string, decimal)>
        {
            ("PETR4", 35), ("VALE3", 30), ("ITUB4", 20), ("ABEV3", 10), ("RENT3", 5)
        });

        _custodiaRepositoryMock
            .Setup(c => c.ObterTodosPorClienteAsync(cliente.Id))
            .ReturnsAsync(new List<Custodia>());

        _cotacaoServiceMock
            .Setup(c => c.ObterCotacaoPorDataAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new Cotacao(DateTime.UtcNow, new Ticker("PETR4"), 35, 35, 36, 34));

        // Act
        var detalhes = await _service.CalcularAjustesAsync(cliente, cestaAnterior, novaCesta);

        // Assert
        Assert.NotNull(detalhes);
        // Verificar que há tickers para comprar (incluindo novos entrados + rebalanceamentos)
        // ABEV3, RENT3 (novos) + PETR4, VALE3 (rebalanceados de maior percentual) = 4
        Assert.True(detalhes.TickersParaComprar.Count >= 2, "Deve ter pelo menos 2 tickers (novos entrados)");
        Assert.Contains("ABEV3", detalhes.TickersParaComprar.Keys.Select(k => k.Valor));
        Assert.Contains("RENT3", detalhes.TickersParaComprar.Keys.Select(k => k.Valor));
        Assert.True(detalhes.TemAlteracoes);
    }

    [Fact]
    public async Task CalcularAjustesAsync_CalculaIRParaVendasAcimaDoLimite()
    {
        // Arrange
        var cliente = new Cliente("Cliente Teste", "12345678901", "teste@teste.com", 100000); // Grande aporte

        var cestaAnterior = new CestaRecomendacao("Top5 Jan", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        var novaCesta = new CestaRecomendacao("Top5 Fev", new List<(string, decimal)>
        {
            ("PETR4", 50), ("VALE3", 25), ("ITUB4", 15), ("ABEV3", 5), ("RENT3", 5)
        });

        // Cliente tem grande posição em tickers que saíram/mudaram
        var custodias = new List<Custodia>
        {
            new Custodia(cliente.Id, new Ticker("BBDC4"), new Quantidade(400), 15.00m),
            new Custodia(cliente.Id, new Ticker("WEGE3"), new Quantidade(300), 40.00m),
            new Custodia(cliente.Id, new Ticker("ITUB4"), new Quantidade(200), 30.00m)
            // Valor total vendas seria: 400*15 + 300*40 + 100*30 = 6000 + 12000 + 3000 = 21.000 > 20.000
        };

        _custodiaRepositoryMock
            .Setup(c => c.ObterTodosPorClienteAsync(cliente.Id))
            .ReturnsAsync(custodias);

        _cotacaoServiceMock
            .Setup(c => c.ObterCotacaoPorDataAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new Cotacao(DateTime.UtcNow, new Ticker("PETR4"), 35, 35, 36, 34));

        // Act
        var detalhes = await _service.CalcularAjustesAsync(cliente, cestaAnterior, novaCesta);

        // Assert
        Assert.NotNull(detalhes);
        Assert.True(detalhes.ValorTotalVendas > 20000, $"Valor total de vendas ({detalhes.ValorTotalVendas}) deve exceder R$ 20.000");
        Assert.True(detalhes.IRAplicado > 0, "IR deve ser calculado (20% sobre vendas > 20.000)");
    }

    [Fact]
    public async Task CalcularAjustesAsync_NaoCalculaIRParaVendasAbaixoDoLimite()
    {
        // Arrange
        var cliente = new Cliente("Cliente Teste", "12345678901", "teste@teste.com", 1000);

        var cestaAnterior = new CestaRecomendacao("Top5 Jan", new List<(string, decimal)>
        {
            ("PETR4", 30), ("VALE3", 25), ("ITUB4", 20), ("BBDC4", 15), ("WEGE3", 10)
        });

        var novaCesta = new CestaRecomendacao("Top5 Fev", new List<(string, decimal)>
        {
            ("PETR4", 35), ("VALE3", 30), ("ITUB4", 25), ("ABEV3", 5), ("RENT3", 5)
        });

        // Cliente tem pequena posição
        var custodias = new List<Custodia>
        {
            new Custodia(cliente.Id, new Ticker("BBDC4"), new Quantidade(10), 15.00m),
            new Custodia(cliente.Id, new Ticker("WEGE3"), new Quantidade(5), 40.00m)
            // Valor total: 10*15 + 5*40 = 150 + 200 = 350 < 20.000
        };

        _custodiaRepositoryMock
            .Setup(c => c.ObterTodosPorClienteAsync(cliente.Id))
            .ReturnsAsync(custodias);

        _cotacaoServiceMock
            .Setup(c => c.ObterCotacaoPorDataAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new Cotacao(DateTime.UtcNow, new Ticker("PETR4"), 35, 35, 36, 34));

        // Act
        var detalhes = await _service.CalcularAjustesAsync(cliente, cestaAnterior, novaCesta);

        // Assert
        Assert.NotNull(detalhes);
        Assert.True(detalhes.ValorTotalVendas < 20000, "Valor total de vendas deve ser menor que R$ 20.000");
        Assert.Equal(0, detalhes.IRAplicado);
    }
}
