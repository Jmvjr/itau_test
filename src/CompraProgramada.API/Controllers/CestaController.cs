using Microsoft.AspNetCore.Mvc;
using MediatR;
using CompraProgramada.Application.DTOs;

namespace CompraProgramada.API.Controllers;

/// <summary>
/// Controller para gerenciar Cestas de Recomendação
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CestaController : ControllerBase
{
    private readonly IMediator _mediator;

    public CestaController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criar nova cesta de recomendação
    /// </summary>
    [HttpPost]
    [ProducesResponseType(statusCode: 201, type: typeof(long))]
    [ProducesResponseType(statusCode: 400)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarCestaRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar command para criar cesta
        await Task.CompletedTask;
        return CreatedAtAction(nameof(Obter), new { id = 0L }, 0L);
    }

    /// <summary>
    /// Obter cesta por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(CestaRecomendacaoDTO))]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Obter(long id, CancellationToken cancellationToken)
    {
        // TODO: Implementar query para obter cesta
        await Task.CompletedTask;
        return NotFound();
    }

    /// <summary>
    /// Listar cestas ativas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        // TODO: Implementar query para listar cestas
        await Task.CompletedTask;
        return Ok(new List<CestaRecomendacaoDTO>());
    }

    /// <summary>
    /// Desativar cesta
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Desativar(long id, CancellationToken cancellationToken)
    {
        // TODO: Implementar command para desativar cesta
        await Task.CompletedTask;
        return Ok();
    }
}

/// <summary>
/// Request para criar cesta
/// </summary>
public class CriarCestaRequest
{
    public string Nome { get; set; } = string.Empty;
    public List<ItemCestaRequest> Itens { get; set; } = new();
}

/// <summary>
/// Item da cesta em request
/// </summary>
public class ItemCestaRequest
{
    public string Ticker { get; set; } = string.Empty;
    public decimal Percentual { get; set; }
}
