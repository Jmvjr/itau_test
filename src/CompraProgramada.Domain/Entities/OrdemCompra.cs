using CompraProgramada.Domain.Enums;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade OrdemCompra — Ordem registrada na conta master
/// RN-031, RN-032, RN-033
/// </summary>
public class OrdemCompra
{
    public long Id { get; private set; }
    public long ContaMasterId { get; private set; }
    public Ticker Ticker { get; private set; } = null!;
    public Quantidade Quantidade { get; private set; } = null!;
    public decimal PrecoUnitario { get; private set; }
    public TipoMercado TipoMercado { get; private set; }
    public DateTime DataExecucao { get; private set; }

    protected OrdemCompra() { }

    public OrdemCompra(long contaMasterId, Ticker ticker, Quantidade quantidade, 
                       decimal precoUnitario, TipoMercado tipoMercado)
    {
        if (quantidade.Valor <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));

        ContaMasterId = contaMasterId;
        Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        TipoMercado = tipoMercado;
        DataExecucao = DateTime.UtcNow;
    }

    /// <summary>
    /// Calcula o valor total da ordem
    /// </summary>
    public decimal CalcularValorTotal() => Quantidade.Valor * PrecoUnitario;

    public override string ToString() => 
        $"{Ticker} {Quantidade} @ R$ {PrecoUnitario:F4} ({TipoMercado}) - Total: R$ {CalcularValorTotal():F2}";
}
