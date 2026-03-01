namespace CompraProgramada.Domain.ValueObjects;

/// <summary>
/// Value Object para Percentual (0-100%)
/// Usado na cesta de recomendação para cada ativo
/// Mapeado como DECIMAL(5,2) no banco de dados conforme ER
/// </summary>
public sealed class Percentual : IEquatable<Percentual>, IComparable<Percentual>
{
    private const decimal MinValue = 0m;
    private const decimal MaxValue = 100m; // Validação de domínio: máximo 100%

    public decimal Valor { get; }

    public Percentual(decimal valor)
    {
        if (valor < MinValue || valor > MaxValue)
            throw new ArgumentException(
                $"Percentual deve estar entre {MinValue}% e {MaxValue}%",
                nameof(valor));

        Valor = valor;
    }

    /// <summary>
    /// Converte percentual para decimal (ex: 30% = 0.30)
    /// </summary>
    public decimal ParaDecimal() => Valor / 100m;

    /// <summary>
    /// Calcula o valor em reais baseado em um total
    /// </summary>
    public decimal CalcularValor(decimal total) => total * ParaDecimal();

    public override string ToString() => $"{Valor:F2}%";

    public override bool Equals(object? obj) => Equals(obj as Percentual);

    public bool Equals(Percentual? other) => other?.Valor == Valor;

    public override int GetHashCode() => Valor.GetHashCode();

    public int CompareTo(Percentual? other) => Valor.CompareTo(other?.Valor);

    public static bool operator ==(Percentual? a, Percentual? b) => (a, b) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        _ => a.Equals(b)
    };

    public static bool operator !=(Percentual? a, Percentual? b) => !(a == b);

    public static bool operator <(Percentual? a, Percentual? b) =>
        a is not null && b is not null && a.Valor < b.Valor;

    public static bool operator >(Percentual? a, Percentual? b) =>
        a is not null && b is not null && a.Valor > b.Valor;

    public static bool operator <=(Percentual? a, Percentual? b) =>
        a is not null && b is not null && a.Valor <= b.Valor;

    public static bool operator >=(Percentual? a, Percentual? b) =>
        a is not null && b is not null && a.Valor >= b.Valor;
}
