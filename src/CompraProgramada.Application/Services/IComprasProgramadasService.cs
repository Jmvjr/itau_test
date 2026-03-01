namespace CompraProgramada.Application.Services;

/// <summary>
/// Serviço de compras programadas — Orquestra o motor de compra
/// Responsável por agrupar, consolidar e executar compras nas datas programadas (5, 15, 25)
/// </summary>
public interface IComprasProgramadasService
{
    /// <summary>
    /// Processa compras programadas para uma data específica
    /// Agrupa clientes, consolida por cesta, verifica saldo e executa compras
    /// </summary>
    /// <param name="dataCompra">Data da compra programada (exemplo: 2026-02-05)</param>
    /// <returns>Número de ordens criadas</returns>
    Task<int> ProcessarComprasAgrupadasAsync(DateTime dataCompra);

    /// <summary>
    /// Retorna as próximas datas de compra programada a partir de hoje
    /// </summary>
    /// <returns>Lista com datas 5, 15 e 25 do mês corrente/próximo (máximo 3 datas)</returns>
    Task<List<DateTime>> ObterProximasDataasCompraAsync();

    /// <summary>
    /// Verifica se há compras pendentes para processar
    /// </summary>
    Task<bool> HaComprasPendentesAsync();
}
