namespace CompraProgramada.Domain.Enums;

/// <summary>
/// Tipo de Evento de Imposto de Renda
/// </summary>
public enum TipoEventoIR
{
    /// <summary>
    /// IR Dedo-Duro (IRRF - 0,005% retido na operação)
    /// </summary>
    DedoDuro = 0,

    /// <summary>
    /// IR sobre lucro em vendas de ações (20% quando aplicável)
    /// </summary>
    IRVenda = 1
}
