using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Value Object ItemCesta — Item da cesta de recomendação
/// </summary>
public class ItemCesta
{
    public long Id { get; private set; }
    public long CestaId { get; private set; }
    public Ticker Ticker { get; private set; } = null!;
    public Percentual Percentual { get; private set; } = null!;

    protected ItemCesta() { }

    public ItemCesta(Ticker ticker, Percentual percentual)
    {
        Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        Percentual = percentual ?? throw new ArgumentNullException(nameof(percentual));
    }

    public override string ToString() => $"{Ticker}: {Percentual}";
}

/// <summary>
/// Entidade CestaRecomendacao — Agregado com 5 ativos (Top Five)
/// RN-014 a RN-019
/// </summary>
public class CestaRecomendacao
{
    private const int QuantidadeAtivosEsperada = 5;
    private const decimal PercentualTotal = 100m;

    public long Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public bool Ativa { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataDesativacao { get; private set; }
    public IReadOnlyList<ItemCesta> Itens => _itens.AsReadOnly();

    private List<ItemCesta> _itens = [];

    protected CestaRecomendacao() { }

    /// <summary>
    /// Criar nova cesta (RN-014, RN-015, RN-016, RN-018)
    /// </summary>
    public CestaRecomendacao(string nome, List<(string ticker, decimal percentual)> itens)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da cesta não pode ser vazio", nameof(nome));

        // RN-014: Exatamente 5 ativos
        if (itens.Count != QuantidadeAtivosEsperada)
            throw new QuantidadeAtivosInvalidaException(itens.Count, QuantidadeAtivosEsperada);

        // RN-015: Soma de percentuais = 100%
        var somaPercentuais = itens.Sum(i => i.percentual);
        if (Math.Abs(somaPercentuais - PercentualTotal) > 0.01m)
            throw new PercentuaisInvalidosException(somaPercentuais);

        // RN-016: Cada percentual > 0%
        if (itens.Any(i => i.percentual <= 0))
            throw new ArgumentException("Cada percentual deve ser maior que 0%");

        Nome = nome;
        Ativa = true;
        DataCriacao = DateTime.UtcNow;

        // Criar items
        _itens = itens
            .Select(i => new ItemCesta(new Ticker(i.ticker), new Percentual(i.percentual)))
            .ToList();
    }

    /// <summary>
    /// Desativar cesta (RN-017)
    /// </summary>
    public void Desativar()
    {
        if (!Ativa)
            throw new InvalidOperationException("Cesta já está desativa");

        Ativa = false;
        DataDesativacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Identifica tickers que saíram da cesta
    /// </summary>
    public List<Ticker> IdentificarTickersSaidos(CestaRecomendacao cestaAnterior)
    {
        var tickersAtuais = _itens.Select(i => i.Ticker).ToHashSet();
        return cestaAnterior.Itens
            .Where(i => !tickersAtuais.Contains(i.Ticker))
            .Select(i => i.Ticker)
            .ToList();
    }

    /// <summary>
    /// Identifica tickers que entraram na cesta
    /// </summary>
    public List<Ticker> IdentificarTickersEntrados(CestaRecomendacao cestaAnterior)
    {
        var tickersAntigos = cestaAnterior.Itens.Select(i => i.Ticker).ToHashSet();
        return _itens
            .Where(i => !tickersAntigos.Contains(i.Ticker))
            .Select(i => i.Ticker)
            .ToList();
    }

    /// <summary>
    /// Obtém o percentual de um ticker específico
    /// </summary>
    public Percentual? ObterPercentualTicker(Ticker ticker)
    {
        return _itens.FirstOrDefault(i => i.Ticker == ticker)?.Percentual;
    }

    public override string ToString() => $"{Nome} ({(_itens.Count)}  ativos) - {(Ativa ? "Ativa" : "Inativa")}";
}
