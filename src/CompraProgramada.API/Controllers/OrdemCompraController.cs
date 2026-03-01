using Microsoft.AspNetCore.Mvc;
using MediatR;
using CompraProgramada.Application.DTOs;

namespace CompraProgramada.API.Controllers;

/// <summary>
/// Controller para gerenciar Ordens de Compra
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdemCompraController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdemCompraController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criar nova ordem de compra
    /// </summary>
    [HttpPost]
    [ProducesResponseType(statusCode: 201, type: typeof(long))]
    [ProducesResponseType(statusCode: 400)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarOrdemCompraRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar command para criar ordem
        await Task.CompletedTask;
        return CreatedAtAction(nameof(Obter), new { id = 0L }, 0L);
    }

    /// <summary>
    /// Obter ordem por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(OrdemCompraDTO))]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Obter(long id, CancellationToken cancellationToken)
    {
        // TODO: Implementar query para obter ordem
        await Task.CompletedTask;
        return NotFound();
    }

    /// <summary>
    /// Listar ordens de compra com filtros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Listar(
        [FromQuery] long? clienteId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implementar query para listar ordens com filtros
        await Task.CompletedTask;
        return Ok(new List<OrdemCompraDTO>());
    }

    /// <summary>
    /// Listar ordens de compra por cliente
    /// </summary>
    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ListarPorCliente(long clienteId, CancellationToken cancellationToken)
    {
        // TODO: Implementar query para listar ordens do cliente
        await Task.CompletedTask;
        return Ok(new List<OrdemCompraDTO>());
    }

    /// <summary>
    /// Listar ordens de compra por data
    /// </summary>
    [HttpGet("por-data")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ListarPorData(
        [FromQuery] DateTime data,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implementar query para listar ordens por data
        await Task.CompletedTask;
        return Ok(new List<OrdemCompraDTO>());
    }
}

/// <summary>
/// Request para criar ordem de compra
/// </summary>
public class CriarOrdemCompraRequest
{
    public long ContaMasterId { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string TipoMercado { get; set; } = "LotePadrao";
}
