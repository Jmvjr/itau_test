namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade Distribuição — Registro de proventos/dividendos distribuídos
/// </summary>
public class Distribuicao
{
    public long Id { get; private set; }
    public long OrdemCompraId { get; private set; }
    public long CustodiaFilhoteId { get; private set; }
    public string Ticker { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public DateTime DataDistribuicao { get; private set; }

    protected Distribuicao() { }

    /// <summary>
    /// Criar nova distribuição
    /// </summary>
    public Distribuicao(long ordemCompraId, long custodiaFilhoteId, string ticker, 
                        int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));

        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker não pode ser vazio", nameof(ticker));

        OrdemCompraId = ordemCompraId;
        CustodiaFilhoteId = custodiaFilhoteId;
        Ticker = ticker;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        DataDistribuicao = DateTime.UtcNow;
    }

    public override string ToString() => $"Distribuição de {Quantidade} x {Ticker} em {DataDistribuicao:dd/MM/yyyy}";
}
