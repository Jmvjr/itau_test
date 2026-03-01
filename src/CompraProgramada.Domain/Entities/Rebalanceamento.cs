using CompraProgramada.Domain.Enums;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade Rebalanceamento — Registro de rebalanceamentos executados
/// Mapeamento ER:
/// - ID: BIGINT AUTO_INCREMENT
/// - ClienteID: BIGINT
/// - ValorVenda: DECIMAL(18,2)
/// - DataRebalanceamento: DATETIME
/// RN-045 a RN-052
/// </summary>
public class Rebalanceamento
{
    public long Id { get; private set; }
    public long ClienteId { get; private set; }
    public TipoRebalanceamento Tipo { get; private set; }
    public Ticker? TickerVendido { get; private set; }
    public Ticker? TickerComprado { get; private set; }
    public decimal ValorVenda { get; private set; } // DECIMAL(18,2) conforme ER
    public DateTime DataRebalanceamento { get; private set; }

    protected Rebalanceamento() { }

    /// <summary>
    /// Criar rebalanceamento por mudança de cesta
    /// </summary>
    public static Rebalanceamento CriarPorMudancaCesta(
        long clienteId, Ticker tickerVendido, Ticker tickerComprado, decimal valorVenda)
    {
        if (valorVenda <= 0)
            throw new ArgumentException("Valor de venda deve ser maior que zero", nameof(valorVenda));

        return new Rebalanceamento
        {
            ClienteId = clienteId,
            Tipo = TipoRebalanceamento.MudancaCesta,
            TickerVendido = tickerVendido ?? throw new ArgumentNullException(nameof(tickerVendido)),
            TickerComprado = tickerComprado ?? throw new ArgumentNullException(nameof(tickerComprado)),
            ValorVenda = valorVenda,
            DataRebalanceamento = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Criar rebalanceamento por desvio de proporção
    /// </summary>
    public static Rebalanceamento CriarPorDesvio(
        long clienteId, Ticker tickerVendido, Ticker tickerComprado, decimal valorVenda)
    {
        if (valorVenda <= 0)
            throw new ArgumentException("Valor de venda deve ser maior que zero", nameof(valorVenda));

        return new Rebalanceamento
        {
            ClienteId = clienteId,
            Tipo = TipoRebalanceamento.Desvio,
            TickerVendido = tickerVendido ?? throw new ArgumentNullException(nameof(tickerVendido)),
            TickerComprado = tickerComprado ?? throw new ArgumentNullException(nameof(tickerComprado)),
            ValorVenda = valorVenda,
            DataRebalanceamento = DateTime.UtcNow
        };
    }

    public override string ToString() => 
        $"{Tipo}: Vender {TickerVendido} (R$ {ValorVenda:F2}) → Comprar {TickerComprado}";
}
