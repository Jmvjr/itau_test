namespace CompraProgramada.Domain.ValueObjects;

/// <summary>
/// Value Object para Quantidade de ações
/// </summary>
public sealed class Quantidade : IEquatable<Quantidade>, IComparable<Quantidade>
{
    private const int MinValue = 0;

    public int Valor { get; }

    public Quantidade(int valor)
    {
        if (valor < MinValue)
            throw new ArgumentException(
                $"Quantidade não pode ser negativa",
                nameof(valor));

        Valor = valor;
    }

    /// <summary>
    /// Verifica se é quantidade válida para lote padrão (múltiplo de 100)
    /// </summary>
    public bool EhLotePadrao => Valor % 100 == 0 && Valor > 0;

    /// <summary>
    /// Retorna quantos lotes padrão (múltiplos de 100) a quantidade tem
    /// </summary>
    public int ObterQuantidadeLotesPadrao() => Valor / 100;

    /// <summary>
    /// Retorna a quantidade fracionária (restante após lotes padrão)
    /// </summary>
    public int ObterQuantidadeFracionaria() => Valor % 100;

    /// <summary>
    /// Adiciona quantidade
    /// </summary>
    public Quantidade Adicionar(Quantidade outra) => new(Valor + outra.Valor);

    /// <summary>
    /// Subtrai quantidade
    /// </summary>
    public Quantidade Subtrair(Quantidade outra)
    {
        var resultado = Valor - outra.Valor;
        if (resultado < 0)
            throw new InvalidOperationException("Quantidade não pode ficar negativa");
        return new Quantidade(resultado);
    }

    public override string ToString() => Valor.ToString();

    public override bool Equals(object? obj) => Equals(obj as Quantidade);

    public bool Equals(Quantidade? other) => other?.Valor == Valor;

    public override int GetHashCode() => Valor.GetHashCode();

    public int CompareTo(Quantidade? other) => Valor.CompareTo(other?.Valor);

    public static bool operator ==(Quantidade? a, Quantidade? b) => (a, b) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        _ => a.Equals(b)
    };

    public static bool operator !=(Quantidade? a, Quantidade? b) => !(a == b);

    public static bool operator <(Quantidade? a, Quantidade? b) =>
        a is not null && b is not null && a.Valor < b.Valor;

    public static bool operator >(Quantidade? a, Quantidade? b) =>
        a is not null && b is not null && a.Valor > b.Valor;

    public static bool operator <=(Quantidade? a, Quantidade? b) =>
        a is not null && b is not null && a.Valor <= b.Valor;

    public static bool operator >=(Quantidade? a, Quantidade? b) =>
        a is not null && b is not null && a.Valor >= b.Valor;

    public static Quantidade operator +(Quantidade a, Quantidade b) => a.Adicionar(b);

    public static Quantidade operator -(Quantidade a, Quantidade b) => a.Subtrair(b);
}
