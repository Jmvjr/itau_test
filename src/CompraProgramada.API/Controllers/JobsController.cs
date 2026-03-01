using Microsoft.AspNetCore.Mvc;
using CompraProgramada.Application.Jobs;

namespace CompraProgramada.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IComprasProgramadasJobService _jobService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(
        IComprasProgramadasJobService jobService,
        ILogger<JobsController> logger)
    {
        _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtém lista de todos os jobs de compras programadas
    /// </summary>
    /// <returns>Lista de jobs registrados com suas informações</returns>
    [HttpGet("compras-programadas")]
    public async Task<IActionResult> ObterJobsComprasProgramadas()
    {
        try
        {
            var jobs = await _jobService.ObterJobsRegistradosAsync();

            return Ok(new
            {
                sucesso = true,
                total = jobs.Count(),
                jobs = jobs.Select(j => new
                {
                    id = j.Id,
                    descricao = j.Descricao,
                    proxima_execucao = j.ProximaExecucao?.ToString("yyyy-MM-dd HH:mm:ss"),
                    ultima_execucao = j.UltimaExecucao?.ToString("yyyy-MM-dd HH:mm:ss"),
                    status = j.Status
                }).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter jobs de compras programadas");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    sucesso = false,
                    erro = "Erro ao obter informações dos jobs",
                    detalhes = ex.Message
                });
        }
    }

    /// <summary>
    /// Processa compras programadas manualmente
    /// Útil para testes e processamento imediato
    /// </summary>
    /// <returns>Resultado do processamento</returns>
    [HttpPost("compras-programadas/executar-agora")]
    public async Task<IActionResult> ExecutarComprasAgora()
    {
        try
        {
            _logger.LogInformation("Execução manual de compras programadas solicitada");

            await _jobService.ProcessarComprasAsync();

            return Ok(new
            {
                sucesso = true,
                mensagem = "Processamento de compras solicitado com sucesso",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar compras programadas manualmente");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    sucesso = false,
                    erro = "Erro ao executar compras programadas",
                    detalhes = ex.Message
                });
        }
    }

    /// <summary>
    /// Verifica saúde do serviço de jobs
    /// </summary>
    /// <returns>Status do serviço</returns>
    [HttpGet("saude")]
    public async Task<IActionResult> VerificarSaude()
    {
        try
        {
            var jobs = await _jobService.ObterJobsRegistradosAsync();

            return Ok(new
            {
                sucesso = true,
                status = "operacional",
                jobs_registrados = jobs.Count(),
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar saúde do serviço");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    sucesso = false,
                    status = "erro",
                    detalhes = ex.Message
                });
        }
    }
}
