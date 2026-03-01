namespace CompraProgramada.Domain.Enums;

/// <summary>
/// Tipo de Rebalanceamento
/// </summary>
public enum TipoRebalanceamento
{
    /// <summary>
    /// Rebalanceamento disparado por mudança da cesta Top Five
    /// </summary>
    MudancaCesta = 0,

    /// <summary>
    /// Rebalanceamento por desvio de proporção da carteira
    /// </summary>
    Desvio = 1
}
