using CompraProgramada.Application.Jobs;
using CompraProgramada.Application.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Infrastructure.Jobs;

/// <summary>
/// Implementação do serviço de jobs usando Hangfire
/// Processa compras programadas automaticamente nas datas agendadas
/// </summary>
public class HangfireComprasProgramadasJobService : IComprasProgramadasJobService
{
    private readonly IComprasProgramadasService _comprasService;
    private readonly ILogger<HangfireComprasProgramadasJobService> _logger;

    // IDs dos jobs para poder acessá-los depois
    private const string JobId5thDay = "compras-programadas-dia-5";
    private const string JobId15thDay = "compras-programadas-dia-15";
    private const string JobId25thDay = "compras-programadas-dia-25";

    public HangfireComprasProgramadasJobService(
        IComprasProgramadasService comprasService,
        ILogger<HangfireComprasProgramadasJobService> logger)
    {
        _comprasService = comprasService ?? throw new ArgumentNullException(nameof(comprasService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registra jobs recorrentes para processar compras nos dias 5, 15 e 25
    /// Usa CRON para agendamento
    /// </summary>
    public async Task RegistrarJobsRecurrentesAsync()
    {
        _logger.LogInformation("Registrando jobs de compras programadas...");

        try
        {
            // Job para dia 5 - 8:00 AM
            // CRON: minuto hora dia mês dia_semana
            // 0 8 5 * * = 08:00 no 5º dia de cada mês
            RecurringJob.AddOrUpdate(
                JobId5thDay,
                () => ProcessarComprasAsync(),
                "0 8 5 * *", // Dia 5 às 8:00
                TimeZoneInfo.Local);

            _logger.LogInformation("Job registrado: {JobId}", JobId5thDay);

            // Job para dia 15 - 8:00 AM
            RecurringJob.AddOrUpdate(
                JobId15thDay,
                () => ProcessarComprasAsync(),
                "0 8 15 * *", // Dia 15 às 8:00
                TimeZoneInfo.Local);

            _logger.LogInformation("Job registrado: {JobId}", JobId15thDay);

            // Job para dia 25 - 8:00 AM
            RecurringJob.AddOrUpdate(
                JobId25thDay,
                () => ProcessarComprasAsync(),
                "0 8 25 * *", // Dia 25 às 8:00
                TimeZoneInfo.Local);

            _logger.LogInformation("Job registrado: {JobId}", JobId25thDay);

            _logger.LogInformation("Todos os jobs de compras programadas foram registrados com sucesso");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar jobs de compras programadas");
            throw;
        }
    }

    /// <summary>
    /// Processa compras programadas para a data atual
    /// Chamado pelo Hangfire nos horários agendados
    /// </summary>
    public async Task ProcessarComprasAsync()
    {
        var dataAtual = DateTime.UtcNow.Date;

        _logger.LogInformation("Iniciando processamento de compras programadas para {Data}", dataAtual);

        try
        {
            // Validar se é data válida (5, 15 ou 25)
            if (dataAtual.Day != 5 && dataAtual.Day != 15 && dataAtual.Day != 25)
            {
                _logger.LogWarning("Data {Data} não é data de compra válida (5, 15 ou 25)", dataAtual);
                return;
            }

            // Processa as compras
            int ordensProcessadas = await _comprasService.ProcessarComprasAgrupadasAsync(dataAtual);

            _logger.LogInformation(
                "Processamento concluído com sucesso. {OrdensProcessadas} ordensprocessadas em {Data}",
                ordensProcessadas,
                dataAtual);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao processar compras para {Data}", dataAtual);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar compras programadas para {Data}", dataAtual);
            throw;
        }
    }

    /// <summary>
    /// Obtém informações sobre jobs registrados
    /// </summary>
    public async Task<IEnumerable<JobInfo>> ObterJobsRegistradosAsync()
    {
        try
        {
            var jobs = new List<JobInfo>
            {
                new JobInfo
                {
                    Id = JobId5thDay,
                    Descricao = "Processamento automático de compras - 5º dia do mês às 8:00",
                    Status = "Ativo"
                },
                new JobInfo
                {
                    Id = JobId15thDay,
                    Descricao = "Processamento automático de compras - 15º dia do mês às 8:00",
                    Status = "Ativo"
                },
                new JobInfo
                {
                    Id = JobId25thDay,
                    Descricao = "Processamento automático de compras - 25º dia do mês às 8:00",
                    Status = "Ativo"
                }
            };

            return await Task.FromResult(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações dos jobs");
            throw;
        }
    }
}
