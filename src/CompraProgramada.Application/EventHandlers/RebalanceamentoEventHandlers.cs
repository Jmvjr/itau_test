using MediatR;
using Microsoft.Extensions.Logging;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Application.Services;

namespace CompraProgramada.Application.EventHandlers;

/// <summary>
/// Evento de aplicação: Cesta foi ativada
/// RN-019: Dispara rebalanceamento de todos os clientes
/// </summary>
public class CestaAtivadaEvent : INotification
{
    public long NovoId { get; set; }
    public string NovoNome { get; set; }
    public long? CestaAnteriorId { get; set; }
    public string? CestaAnteriorNome { get; set; }
    public DateTime DataAtivacao { get; set; }
    public List<(string Ticker, decimal Percentual)> Itens { get; set; }

    public CestaAtivadaEvent(
        long novoId,
        string novoNome,
        long? cestaAnteriorId,
        string? cestaAnteriorNome,
        List<(string, decimal)> itens)
    {
        NovoId = novoId;
        NovoNome = novoNome;
        CestaAnteriorId = cestaAnteriorId;
        CestaAnteriorNome = cestaAnteriorNome;
        DataAtivacao = DateTime.UtcNow;
        Itens = itens;
    }
}

/// <summary>
/// Evento de aplicação: Rebalanceamento foi concluído para um cliente
/// </summary>
public class RebalanceamentoConcluidoEvent : INotification
{
    public long ClienteId { get; set; }
    public string ClienteNome { get; set; }
    public int TickersParaVender { get; set; }
    public int TickersParaComprar { get; set; }
    public int TickersParaRebalancear { get; set; }
    public decimal ValorTotalVendas { get; set; }
    public decimal IRAplicado { get; set; }
    public DateTime DataRebalanceamento { get; set; }

    public RebalanceamentoConcluidoEvent(
        long clienteId,
        string clienteNome,
        int tickersParaVender,
        int tickersParaComprar,
        int tickersParaRebalancear,
        decimal valorTotalVendas,
        decimal irAplicado)
    {
        ClienteId = clienteId;
        ClienteNome = clienteNome;
        TickersParaVender = tickersParaVender;
        TickersParaComprar = tickersParaComprar;
        TickersParaRebalancear = tickersParaRebalancear;
        ValorTotalVendas = valorTotalVendas;
        IRAplicado = irAplicado;
        DataRebalanceamento = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento de aplicação: Venda foi realizada durante rebalanceamento
/// </summary>
public class VendaRebalanceamentoEvent : INotification
{
    public long ClienteId { get; set; }
    public string Ticker { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string Motivo { get; set; } // "Ticker saiu", "Redução percentual", etc
    public DateTime DataVenda { get; set; }

    public VendaRebalanceamentoEvent(
        long clienteId,
        string ticker,
        int quantidade,
        decimal precoUnitario,
        string motivo)
    {
        ClienteId = clienteId;
        Ticker = ticker;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        ValorTotal = quantidade * precoUnitario;
        Motivo = motivo;
        DataVenda = DateTime.UtcNow;
    }
}

/// <summary>
/// Evento de aplicação: Compra foi realizada durante rebalanceamento
/// </summary>
public class CompraRebalanceamentoEvent : INotification
{
    public long ClienteId { get; set; }
    public string Ticker { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string Motivo { get; set; } // "Ticker novo", "Aumento percentual", etc
    public DateTime DataCompra { get; set; }

    public CompraRebalanceamentoEvent(
        long clienteId,
        string ticker,
        int quantidade,
        decimal precoUnitario,
        string motivo)
    {
        ClienteId = clienteId;
        Ticker = ticker;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        ValorTotal = quantidade * precoUnitario;
        Motivo = motivo;
        DataCompra = DateTime.UtcNow;
    }
}

/// <summary>
/// Handler para CestaAtivadaEvent
/// Publica evento no Kafka e registra no audit trail
/// RN-019: Quando cesta muda, dispara rebalanceamento
/// </summary>
public class CestaAtivadaEventHandler : INotificationHandler<CestaAtivadaEvent>
{
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<CestaAtivadaEventHandler> _logger;

    public CestaAtivadaEventHandler(
        IKafkaProducerService kafkaProducer,
        ILogger<CestaAtivadaEventHandler> logger)
    {
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(CestaAtivadaEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Cesta ativada: ID={NovoId} Nome={NovoNome} Data={DataAtivacao}",
            @event.NovoId,
            @event.NovoNome,
            @event.DataAtivacao);

        // Publicar evento no topic Kafka
        var mensagem = new
        {
            @event.NovoId,
            @event.NovoNome,
            @event.CestaAnteriorId,
            @event.CestaAnteriorNome,
            @event.DataAtivacao,
            @event.Itens
        };

        await _kafkaProducer.PublicarAsync("cesta-ativada", "cesta", mensagem);
    }
}

/// <summary>
/// Handler para RebalanceamentoConcluidoEvent
/// Publica evento no Kafka e registra no audit trail
/// </summary>
public class RebalanceamentoConcluidoEventHandler : INotificationHandler<RebalanceamentoConcluidoEvent>
{
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<RebalanceamentoConcluidoEventHandler> _logger;

    public RebalanceamentoConcluidoEventHandler(
        IKafkaProducerService kafkaProducer,
        ILogger<RebalanceamentoConcluidoEventHandler> logger)
    {
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(RebalanceamentoConcluidoEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Rebalanceamento concluído: Cliente={ClienteId} ({ClienteNome}) - Vendas={TickersParaVender} Compras={TickersParaComprar} Rebalanceamentos={TickersParaRebalancear} Valor={ValorTotalVendas:C} IR={IRAplicado:C}",
            @event.ClienteId,
            @event.ClienteNome,
            @event.TickersParaVender,
            @event.TickersParaComprar,
            @event.TickersParaRebalancear,
            @event.ValorTotalVendas,
            @event.IRAplicado);

        // Publicar evento no topic Kafka
        var mensagem = new
        {
            @event.ClienteId,
            @event.ClienteNome,
            @event.TickersParaVender,
            @event.TickersParaComprar,
            @event.TickersParaRebalancear,
            @event.ValorTotalVendas,
            @event.IRAplicado,
            @event.DataRebalanceamento
        };

        await _kafkaProducer.PublicarAsync("rebalanceamento-concluido", "cliente-" + @event.ClienteId, mensagem);
    }
}

/// <summary>
/// Handler para VendaRebalanceamentoEvent
/// Publica evento no Kafka e registra no audit trail
/// RN-039: Venda de ativos durante rebalanceamento
/// </summary>
public class VendaRebalanceamentoEventHandler : INotificationHandler<VendaRebalanceamentoEvent>
{
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<VendaRebalanceamentoEventHandler> _logger;

    public VendaRebalanceamentoEventHandler(
        IKafkaProducerService kafkaProducer,
        ILogger<VendaRebalanceamentoEventHandler> logger)
    {
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(VendaRebalanceamentoEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Venda rebalanceamento: Cliente={ClienteId} Ticker={Ticker} Qtd={Quantidade} Preço={PrecoUnitario:C} Total={ValorTotal:C} Motivo={Motivo}",
            @event.ClienteId,
            @event.Ticker,
            @event.Quantidade,
            @event.PrecoUnitario,
            @event.ValorTotal,
            @event.Motivo);

        // Publicar evento no topic Kafka
        var mensagem = new
        {
            @event.ClienteId,
            @event.Ticker,
            @event.Quantidade,
            @event.PrecoUnitario,
            @event.ValorTotal,
            @event.Motivo,
            @event.DataVenda
        };

        await _kafkaProducer.PublicarAsync("rebalanceamento-vendas", "cliente-" + @event.ClienteId, mensagem);
    }
}

/// <summary>
/// Handler para CompraRebalanceamentoEvent
/// Publica evento no Kafka e registra no audit trail
/// RN-018: Compra de novos ativos durante rebalanceamento
/// </summary>
public class CompraRebalanceamentoEventHandler : INotificationHandler<CompraRebalanceamentoEvent>
{
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<CompraRebalanceamentoEventHandler> _logger;

    public CompraRebalanceamentoEventHandler(
        IKafkaProducerService kafkaProducer,
        ILogger<CompraRebalanceamentoEventHandler> logger)
    {
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(CompraRebalanceamentoEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Compra rebalanceamento: Cliente={ClienteId} Ticker={Ticker} Qtd={Quantidade} Preço={PrecoUnitario:C} Total={ValorTotal:C} Motivo={Motivo}",
            @event.ClienteId,
            @event.Ticker,
            @event.Quantidade,
            @event.PrecoUnitario,
            @event.ValorTotal,
            @event.Motivo);

        // Publicar evento no topic Kafka
        var mensagem = new
        {
            @event.ClienteId,
            @event.Ticker,
            @event.Quantidade,
            @event.PrecoUnitario,
            @event.ValorTotal,
            @event.Motivo,
            @event.DataCompra
        };

        await _kafkaProducer.PublicarAsync("rebalanceamento-compras", "cliente-" + @event.ClienteId, mensagem);
    }
}
