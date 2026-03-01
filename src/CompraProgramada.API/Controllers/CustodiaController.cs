using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace CompraProgramada.API.Controllers;

/// <summary>
/// Controller para gerenciar Custódias
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustodiaController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustodiaController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Obter custódia por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Obter(long id, CancellationToken cancellationToken)
    {
        // TODO: Implementar query para obter custódia
        await Task.CompletedTask;
        return NotFound();
    }

    /// <summary>
    /// Listar custódias por conta gráfica
    /// </summary>
    [HttpGet("conta/{contaGraficaId}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ListarPorConta(long contaGraficaId, CancellationToken cancellationToken)
    {
        // TODO: Implementar query para listar custódias da conta
        await Task.CompletedTask;
        return Ok(new List<object>());
    }

    /// <summary>
    /// Obter custódia por conta e ticker
    /// </summary>
    [HttpGet("conta/{contaGraficaId}/ticker/{ticker}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ObterPorContaETicker(long contaGraficaId, string ticker, CancellationToken cancellationToken)
    {
        // TODO: Implementar query para obter custódia por conta e ticker
        await Task.CompletedTask;
        return NotFound();
    }

    /// <summary>
    /// Listar todas as custódias com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implementar query para listar todas as custódias
        await Task.CompletedTask;
        return Ok(new List<object>());
    }
}
