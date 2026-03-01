namespace CompraProgramada.Domain.ValueObjects;

/// <summary>
/// Value Object para Ticker (código de ação na B3)
/// Exemplo: PETR4, VALE3, ITUB4, PETR4F (fracionário)
/// Tamanho: 1-10 caracteres (VARCHAR(10) conforme ER)
/// </summary>
public sealed class Ticker : IEquatable<Ticker>
{
    private const int TickerMinLength = 1;
    private const int TickerMaxLength = 10; // VARCHAR(10) conforme ER

    public string Valor { get; }

    public Ticker(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Ticker não pode ser vazio", nameof(valor));

        var tickerLimpo = valor.ToUpper().Trim();

        if (tickerLimpo.Length < TickerMinLength || tickerLimpo.Length > TickerMaxLength)
            throw new ArgumentException(
                $"Ticker deve ter entre {TickerMinLength} e {TickerMaxLength} caracteres",
                nameof(valor));

        // Validar se contém apenas letras e números
        if (!tickerLimpo.All(char.IsLetterOrDigit))
            throw new ArgumentException("Ticker deve conter apenas letras e números", nameof(valor));

        Valor = tickerLimpo;
    }

    /// <summary>
    /// Verifica se é um ticker fracionário (termina com 'F')
    /// </summary>
    public bool EhFracionario => Valor.EndsWith('F');

    /// <summary>
    /// Retorna o ticker do lote padrão (remove 'F' se fracionário)
    /// </summary>
    public Ticker ObterTikerLotePadrao()
    {
        if (EhFracionario)
            return new Ticker(Valor[..^1]); // Remove último caractere (F)
        return this;
    }

    public override string ToString() => Valor;

    public override bool Equals(object? obj) => Equals(obj as Ticker);

    public bool Equals(Ticker? other) => other?.Valor == Valor;

    public override int GetHashCode() => Valor.GetHashCode();

    public static bool operator ==(Ticker? a, Ticker? b) => (a, b) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        _ => a.Equals(b)
    };

    public static bool operator !=(Ticker? a, Ticker? b) => !(a == b);
}
