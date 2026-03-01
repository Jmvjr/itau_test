using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade Custodia — Posição de ações em uma conta
/// RN-041, RN-042, RN-043, RN-044
/// </summary>
public class Custodia
{
    public long Id { get; private set; }
    public long ContaGraficaId { get; private set; }
    public Ticker Ticker { get; private set; } = null!;
    public Quantidade Quantidade { get; private set; } = null!;
    public decimal PrecoMedio { get; private set; }
    public DateTime DataUltimaAtualizacao { get; private set; }

    // Navegação
    public ContaGrafica? ContaGrafica { get; private set; }

    protected Custodia() { }

    /// <summary>
    /// Criar nova posição em custodia
    /// </summary>
    public Custodia(long contaGraficaId, Ticker ticker, Quantidade quantidade, decimal precoMedio)
    {
        if (quantidade.Valor <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        if (precoMedio <= 0)
            throw new ArgumentException("Preço médio deve ser maior que zero", nameof(precoMedio));

        ContaGraficaId = contaGraficaId;
        Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        Quantidade = quantidade;
        PrecoMedio = precoMedio;
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualizar preço médio com nova compra (RN-042)
    /// PM = (Qtd Anterior x PM Anterior + Qtd Nova x Preco Nova) / (Qtd Anterior + Qtd Nova)
    /// </summary>
    public void AtualizarComNovaCompra(Quantidade qtdAdicionada, decimal precoNovaCompra)
    {
        if (qtdAdicionada.Valor <= 0)
            throw new ArgumentException("Quantidade adicionada deve ser maior que zero", nameof(qtdAdicionada));

        if (precoNovaCompra <= 0)
            throw new ArgumentException("Preço da nova compra deve ser maior que zero", nameof(precoNovaCompra));

        var novoPrecoMedio = (Quantidade.Valor * PrecoMedio + qtdAdicionada.Valor * precoNovaCompra) 
                             / (Quantidade.Valor + qtdAdicionada.Valor);

        Quantidade = Quantidade.Adicionar(qtdAdicionada);
        PrecoMedio = Math.Round(novoPrecoMedio, 4); // Arredondar para 4 casas decimais
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Vender ações (apenas diminui quantidade, preço médio permanece RN-043)
    /// </summary>
    public void Vender(Quantidade qtdVendida)
    {
        if (qtdVendida.Valor <= 0)
            throw new ArgumentException("Quantidade vendida deve ser maior que zero", nameof(qtdVendida));

        if (qtdVendida.Valor > Quantidade.Valor)
            throw new InvalidOperationException("Quantidade vendida não pode exceder a posição");

        Quantidade = Quantidade.Subtrair(qtdVendida);
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Calcula o valor total investido nessa posição
    /// </summary>
    public decimal CalcularValorInvestido() => Quantidade.Valor * PrecoMedio;

    /// <summary>
    /// Calcula o valor atual da posição (quantidade x cotação)
    /// </summary>
    public decimal CalcularValorAtual(decimal cotacaoAtual) => Quantidade.Valor * cotacaoAtual;

    /// <summary>
    /// Calcula o lucro/prejuízo da posição
    /// P/L = (Cotação Atual - Preço Médio) x Quantidade
    /// </summary>
    public decimal CalcularPL(decimal cotacaoAtual) => (cotacaoAtual - PrecoMedio) * Quantidade.Valor;

    public override string ToString() => $"{Ticker}: {Quantidade} @ R$ {PrecoMedio:F4}";
}
