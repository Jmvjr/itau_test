using Microsoft.AspNetCore.Mvc;
using CompraProgramada.Application.Services;

namespace CompraProgramada.API.Controllers;

[ApiController]
[Route("api/compra-programada")]
public class ComprasProgramadaController : ControllerBase
{
    private readonly IComprasProgramadasService _comprasService;
    private readonly ILogger<ComprasProgramadaController> _logger;

    public ComprasProgramadaController(
        IComprasProgramadasService comprasService,
        ILogger<ComprasProgramadaController> logger)
    {
        _comprasService = comprasService ?? throw new ArgumentNullException(nameof(comprasService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processa compras programadas para uma data específica (5, 15 ou 25 do mês)
    /// </summary>
    /// <param name="data">Data no formato yyyy-MM-dd (deve ser 5, 15 ou 25)</param>
    /// <returns>Resultado do processamento com número de ordens criadas</returns>
    [HttpPost("processar/{data:datetime}")]
    public async Task<IActionResult> ProcessarCompras(DateTime data)
    {
        try
        {
            _logger.LogInformation("Iniciando processamento de compras programadas para {Data}", data);

            int ordensProcessadas = await _comprasService.ProcessarComprasAgrupadasAsync(data);

            _logger.LogInformation("Processamento concluído: {OrdensProcessadas} ordens criadas", ordensProcessadas);

            return Ok(new
            {
                sucesso = true,
                ordensProcessadas = ordensProcessadas,
                mensagem = $"Processamento concluído com {ordensProcessadas} ordem(ns) criadas.",
                data = data.ToString("yyyy-MM-dd")
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Argumento inválido: {Mensagem}", ex.Message);
            return BadRequest(new
            {
                sucesso = false,
                erro = ex.Message,
                data = data.ToString("yyyy-MM-dd")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar compras programadas");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    sucesso = false,
                    erro = "Erro ao processar compras programadas",
                    detalhes = ex.Message
                });
        }
    }

    /// <summary>
    /// Obtém as próximas datas de compra programada (5, 15, 25)
    /// </summary>
    /// <returns>Lista com as próximas 3 datas de compra</returns>
    [HttpGet("proximas-datas")]
    public async Task<IActionResult> ObterProximasDatas()
    {
        try
        {
            var datas = await _comprasService.ObterProximasDataasCompraAsync();

            return Ok(new
            {
                sucesso = true,
                proximas_datas = datas.Select(d => d.ToString("yyyy-MM-dd")).ToList(),
                quantidade = datas.Count,
                mensagem = "Próximas datas de compra programada"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter próximas datas de compra");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    sucesso = false,
                    erro = "Erro ao obter próximas datas",
                    detalhes = ex.Message
                });
        }
    }

    /// <summary>
    /// Verifica se há compras pendentes a processar
    /// </summary>
    /// <returns>Indicador de compras pendentes</returns>
    [HttpGet("ha-pendentes")]
    public async Task<IActionResult> VerificarComprasPendentes()
    {
        try
        {
            bool temPendentes = await _comprasService.HaComprasPendentesAsync();

            return Ok(new
            {
                sucesso = true,
                tem_pendentes = temPendentes,
                mensagem = temPendentes
                    ? "Existem compras pendentes a processar"
                    : "Não há compras pendentes"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar compras pendentes");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    sucesso = false,
                    erro = "Erro ao verificar compras pendentes",
                    detalhes = ex.Message
                });
        }
    }
}
