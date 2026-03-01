namespace CompraProgramada.Domain.Entities;

using CompraProgramada.Domain.ValueObjects;

/// <summary>
/// Entidade Distribuição — Registro de proventos/dividendos distribuídos
/// </summary>
public class Distribuicao
{
    public long Id { get; private set; }
    public long? OrdemCompraId { get; private set; }
    public long ClienteId { get; private set; }
    public long ContaMasterId { get; private set; }
    public Ticker Ticker { get; private set; } = null!;
    public Quantidade Quantidade { get; private set; } = null!;
    public DateTime DataDistribuicao { get; private set; }

    protected Distribuicao() { }

    /// <summary>
    /// Criar nova distribuição (versão original com ordem de compra)
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
        ClienteId = custodiaFilhoteId;
        Ticker = new Ticker(ticker);
        Quantidade = new Quantidade((int)quantidade);
        DataDistribuicao = DateTime.UtcNow;
    }

    /// <summary>
    /// Criar nova distribuição para clientes (versão para compra programada)
    /// </summary>
    public Distribuicao(long clienteId, long contaMasterId, Ticker ticker, 
                        Quantidade quantidade, DateTime dataDistribuicao)
    {
        if (clienteId <= 0)
            throw new ArgumentException("ID do cliente deve ser válido", nameof(clienteId));

        if (contaMasterId <= 0)
            throw new ArgumentException("ID da conta master deve ser válido", nameof(contaMasterId));

        ClienteId = clienteId;
        ContaMasterId = contaMasterId;
        Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        Quantidade = quantidade ?? throw new ArgumentNullException(nameof(quantidade));
        DataDistribuicao = dataDistribuicao;
    }

    public override string ToString() => $"Distribuição de {Quantidade} x {Ticker} em {DataDistribuicao:dd/MM/yyyy}";
}
