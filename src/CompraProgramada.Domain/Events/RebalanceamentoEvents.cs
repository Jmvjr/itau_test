using MediatR;
using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Domain.Events;

/// <summary>
/// Evento de domínio: Cesta foi ativada
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
/// Evento de domínio: Rebalanceamento foi concluído para um cliente
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
/// Evento de domínio: Venda foi realizada durante rebalanceamento
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
/// Evento de domínio: Compra foi realizada durante rebalanceamento
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
