namespace CompraProgramada.Domain.Enums;

/// <summary>
/// Tipo de Conta Gráfica
/// </summary>
public enum TipoConta
{
    /// <summary>
    /// Conta Master da corretora (consolidada)
    /// </summary>
    Master = 0,

    /// <summary>
    /// Conta Filhote do cliente individual
    /// </summary>
    Filhote = 1
}
