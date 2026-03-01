using CompraProgramada.Domain.Enums;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade ContaGrafica — Conta de registro escritural do cliente
/// RN-004: Criada automaticamente ao cliente aderir ao produto
/// </summary>
public class ContaGrafica
{
    public long Id { get; private set; }
    public long? ClienteId { get; private set; }
    public string NumeroConta { get; private set; } = string.Empty;
    public TipoConta Tipo { get; private set; }
    public DateTime DataCriacao { get; private set; }

    // Navegação
    public Cliente? Cliente { get; private set; }

    protected ContaGrafica() { }

    /// <summary>
    /// Criar conta filhote para um cliente
    /// </summary>
    public static ContaGrafica CriarContaFilhote(long clienteId, string numeroConta)
    {
        if (string.IsNullOrWhiteSpace(numeroConta))
            throw new ArgumentException("Número da conta não pode ser vazio", nameof(numeroConta));

        return new ContaGrafica
        {
            ClienteId = clienteId,
            NumeroConta = numeroConta,
            Tipo = TipoConta.Filhote,
            DataCriacao = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Criar conta master (sem cliente vinculado)
    /// </summary>
    public static ContaGrafica CriarContaMaster(string numeroConta)
    {
        if (string.IsNullOrWhiteSpace(numeroConta))
            throw new ArgumentException("Número da conta não pode ser vazio", nameof(numeroConta));

        return new ContaGrafica
        {
            ClienteId = null,
            NumeroConta = numeroConta,
            Tipo = TipoConta.Master,
            DataCriacao = DateTime.UtcNow
        };
    }

    public override string ToString() => $"{Tipo} - {NumeroConta}";
}
