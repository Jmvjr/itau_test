namespace CompraProgramada.Domain.ValueObjects;

/// <summary>
/// Value Object para CPF (11 dígitos, único)
/// </summary>
public sealed class CPF : IEquatable<CPF>
{
    private const int CpfLength = 11;

    public string Valor { get; }

    public CPF(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("CPF não pode ser vazio", nameof(valor));

        // Remover formatação (pontos, hífens)
        var cpfLimpo = valor.Replace(".", "").Replace("-", "").Trim();

        if (cpfLimpo.Length != CpfLength || !cpfLimpo.All(char.IsDigit))
            throw new ArgumentException($"CPF deve conter exatamente {CpfLength} dígitos", nameof(valor));

        // Validar se não é sequência repetida (111.111.111-11, 000.000.000-00) 
        // Regra da receita federal para CPF inválido
        if (cpfLimpo.Distinct().Count() == 1)
            throw new ArgumentException("CPF inválido (sequência repetida)", nameof(valor));

        Valor = cpfLimpo;
    }

    public override string ToString() => Valor;

    public override bool Equals(object? obj) => Equals(obj as CPF);

    public bool Equals(CPF? other) => other?.Valor == Valor;

    public override int GetHashCode() => Valor.GetHashCode();

    public static bool operator ==(CPF? a, CPF? b) => (a, b) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        _ => a.Equals(b)
    };

    public static bool operator !=(CPF? a, CPF? b) => !(a == b);
}
