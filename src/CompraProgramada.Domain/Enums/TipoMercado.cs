namespace CompraProgramada.Domain.Enums;

/// <summary>
/// Tipo de Mercado para negociação
/// </summary>
public enum TipoMercado
{
    /// <summary>
    /// Lote Padrão (múltiplos de 100)
    /// </summary>
    LotePadrao = 0,

    /// <summary>
    /// Mercado Fracionário (1-99 ações)
    /// </summary>
    Fracionario = 1
}
