using Microsoft.AspNetCore.Mvc;
using MediatR;
using CompraProgramada.Application.DTOs;
using CompraProgramada.Application.Commands;
using CompraProgramada.Application.Queries;

namespace CompraProgramada.API.Controllers;

/// <summary>
/// Controller para gerenciar Cestas de Recomendação
/// RN-014 a RN-019
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
    /// RN-014: Exatamente 5 ativos
    /// RN-015: Soma percentuais = 100%
    /// RN-016: Cada percentual > 0%
    /// </summary>
    [HttpPost]
    [ProducesResponseType(statusCode: 201, type: typeof(long))]
    [ProducesResponseType(statusCode: 400)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarCestaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var itens = request.Itens.Select(i => (i.Ticker, i.Percentual)).ToList();
            var command = new CriarCestaCommand(request.Nome, itens);
            var cestaId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Obter), new { id = cestaId }, cestaId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = ex.Message });
        }
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
        try
        {
            var query = new ObterCestaPorIdQuery(id);
            var cesta = await _mediator.Send(query, cancellationToken);
            
            if (cesta == null)
                return NotFound(new { mensagem = $"Cesta {id} não encontrada" });
            
            return Ok(cesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Listar cestas ativas
    /// </summary>
    [HttpGet("ativas")]
    [ProducesResponseType(statusCode: 200, type: typeof(List<CestaRecomendacaoDTO>))]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ListarAtivas(CancellationToken cancellationToken)
    {
        try
        {
            var query = new ListarCestasAtivasQuery();
            var cestas = await _mediator.Send(query, cancellationToken);
            return Ok(cestas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Listar todas as cestas (ativas e inativas)
    /// </summary>
    [HttpGet("todas")]
    [ProducesResponseType(statusCode: 200, type: typeof(List<CestaRecomendacaoDTO>))]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ListarTodas(CancellationToken cancellationToken)
    {
        try
        {
            var query = new ListarTodasAsCestasQuery();
            var cestas = await _mediator.Send(query, cancellationToken);
            return Ok(cestas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Obter cesta ativa atualmente
    /// </summary>
    [HttpGet("ativa")]
    [ProducesResponseType(statusCode: 200, type: typeof(CestaRecomendacaoDTO))]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ObterAtiva(CancellationToken cancellationToken)
    {
        try
        {
            var query = new ObterCestaAtivaQuery();
            var cesta = await _mediator.Send(query, cancellationToken);
            
            if (cesta == null)
                return NotFound(new { mensagem = "Nenhuma cesta ativa no momento" });
            
            return Ok(cesta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Ativar uma cesta (desativa a anterior e dispara rebalanceamento)
    /// RN-017: Desativa cesta anterior
    /// RN-019: Dispara rebalanceamento de todos os clientes
    /// </summary>
    [HttpPut("{id}/ativar")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Ativar(long id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new AtivarCestaCommand(id);
            await _mediator.Send(command, cancellationToken);
            return Ok(new { mensagem = $"Cesta {id} ativada com sucesso e rebalanceamento disparado" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Desativar cesta
    /// RN-017
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Desativar(long id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DesativarCestaCommand(id);
            await _mediator.Send(command, cancellationToken);
            return Ok(new { mensagem = $"Cesta {id} desativada com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = ex.Message });
        }
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
