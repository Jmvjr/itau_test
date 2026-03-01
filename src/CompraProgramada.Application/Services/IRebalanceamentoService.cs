using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Application.Services;

/// <summary>
/// Serviço de rebalanceamento de carteira
/// RN-019: Disparado quando a cesta de recomendação muda
/// 
/// Responsabilidades:
/// - Identificar tickers que entraram/saíram/mudaram de percentual
/// - Gerar ordens de venda para tickers que saíram
/// - Gerar ordens de compra para tickers que entraram
/// - Rebalancear proporções para tickers que mudaram percentual
/// - Calcular IR (20%) quando vendas excedem R$ 20.000/mês
/// - Atualizar preço médio das custódias
/// </summary>
public interface IRebalanceamentoService
{
    /// <summary>
    /// Processa rebalanceamento de todos os clientes ativos
    /// Disparado quando uma nova cesta é ativada
    /// </summary>
    /// <param name="cestaAnterior">Cesta desativada</param>
    /// <param name="novaCesta">Cesta recém-ativada</param>
    /// <returns>Número de operações de rebalanceamento realizadas</returns>
    Task<int> ProcessarRebalanceamentoAsync(CestaRecomendacao cestaAnterior, CestaRecomendacao novaCesta);

    /// <summary>
    /// Calcula ajustes necessários para um cliente específico
    /// </summary>
    /// <param name="cliente">Cliente a rebalancear</param>
    /// <param name="cestaAnterior">Cesta antiga</param>
    /// <param name="novaCesta">Cesta nova</param>
    /// <returns>Detalhes sobre vendas, compras e IR aplicado</returns>
    Task<DetalhesRebalanceamento> CalcularAjustesAsync(
        Cliente cliente,
        CestaRecomendacao cestaAnterior,
        CestaRecomendacao novaCesta);
}

/// <summary>
/// Resultado do cálculo de rebalanceamento para um cliente
/// </summary>
public class DetalhesRebalanceamento
{
    /// <summary>Tickers para vender com quantidades</summary>
    public Dictionary<Ticker, int> TickersParaVender { get; set; } = [];

    /// <summary>Tickers para comprar com quantidades</summary>
    public Dictionary<Ticker, int> TickersParaComprar { get; set; } = [];

    /// <summary>Tickers para rebalancear com novas proporções</summary>
    public Dictionary<Ticker, int> TickersParaRebalancear { get; set; } = [];

    /// <summary>Valor total das vendas em R$</summary>
    public decimal ValorTotalVendas { get; set; }

    /// <summary>IR incidido (20% se vendas > 20.000)</summary>
    public decimal IRAplicado { get; set; }

    /// <summary>Indica se é necessário rebalanceamento</summary>
    public bool TemAlteracoes => TickersParaVender.Any() || TickersParaComprar.Any() || TickersParaRebalancear.Any();
}
