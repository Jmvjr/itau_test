using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade Cotacao — Preço de abertura, fechamento, máx, mín de um ativo num dia
/// Vem do arquivo COTAHIST da B3
/// </summary>
public class Cotacao
{
    public long Id { get; private set; }
    public DateTime DataPregao { get; private set; }
    public Ticker Ticker { get; private set; } = null!;
    public decimal PrecoAbertura { get; private set; }
    public decimal PrecoFechamento { get; private set; }
    public decimal PrecoMaximo { get; private set; }
    public decimal PrecoMinimo { get; private set; }

    protected Cotacao() { }

    public Cotacao(DateTime dataPregao, Ticker ticker, decimal precoAbertura, 
                   decimal precoFechamento, decimal precoMaximo, decimal precoMinimo)
    {
        if (precoAbertura <= 0 || precoFechamento <= 0 || precoMaximo <= 0 || precoMinimo <= 0)
            throw new ArgumentException("Preços devem ser maiores que zero");

        if (precoMaximo < precoMinimo)
            throw new ArgumentException("Preço máximo não pode ser menor que mínimo");

        DataPregao = dataPregao;
        Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        PrecoAbertura = precoAbertura;
        PrecoFechamento = precoFechamento;
        PrecoMaximo = precoMaximo;
        PrecoMinimo = precoMinimo;
    }

    public override string ToString() => 
        $"{Ticker} - {DataPregao:yyyy-MM-dd}: Fechamento R$ {PrecoFechamento:F4}";
}
