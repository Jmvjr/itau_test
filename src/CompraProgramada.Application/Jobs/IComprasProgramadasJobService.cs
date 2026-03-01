using CompraProgramada.Application.Services;

namespace CompraProgramada.Application.Jobs;

/// <summary>
/// Interface para agendamento de jobs de compras programadas
/// Abstrai a implementação específica (Hangfire, Quartz, etc)
/// </summary>
public interface IComprasProgramadasJobService
{
    /// <summary>
    /// Registra job recorrente para processamento de compras nas datas 5, 15, 25
    /// </summary>
    Task RegistrarJobsRecurrentesAsync();

    /// <summary>
    /// Processa compras programadas (chamado pelo job)
    /// </summary>
    Task ProcessarComprasAsync();

    /// <summary>
    /// Obtém lista de jobs registrados
    /// </summary>
    Task<IEnumerable<JobInfo>> ObterJobsRegistradosAsync();
}

/// <summary>
/// Informações sobre um job agendado
/// </summary>
public class JobInfo
{
    public string Id { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime? ProximaExecucao { get; set; }
    public DateTime? UltimaExecucao { get; set; }
    public string Status { get; set; } = "Aguardando";
}
