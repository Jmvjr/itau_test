using Microsoft.AspNetCore.Mvc;
using MediatR;
using CompraProgramada.Application.Commands;
using CompraProgramada.Application.Queries;
using CompraProgramada.Application.DTOs;

namespace CompraProgramada.API.Controllers;

/// <summary>
/// Controller para gerenciar Clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClienteController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criar novo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(statusCode: 201, type: typeof(long))]
    [ProducesResponseType(statusCode: 400)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CriarClienteCommand(
            request.Nome,
            request.CPF,
            request.Email,
            request.ValorMensal);

        var clienteId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = clienteId }, clienteId);
    }

    /// <summary>
    /// Obter cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(ClienteDTO))]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ObterPorId(long id, CancellationToken cancellationToken)
    {
        var query = new ObterClientePorIdQuery(id);
        var cliente = await _mediator.Send(query, cancellationToken);

        if (cliente is null)
            return NotFound($"Cliente com ID {id} não encontrado");

        return Ok(cliente);
    }

    /// <summary>
    /// Obter cliente por CPF
    /// </summary>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(statusCode: 200, type: typeof(ClienteDTO))]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ObterPorCpf(string cpf, CancellationToken cancellationToken)
    {
        var query = new ObterClientePorCpfQuery(cpf);
        var cliente = await _mediator.Send(query, cancellationToken);

        if (cliente is null)
            return NotFound($"Cliente com CPF {cpf} não encontrado");

        return Ok(cliente);
    }

    /// <summary>
    /// Listar clientes ativos com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(statusCode: 200, type: typeof(ListarClientesDTO))]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new ListarClientesAtivosQuery 
        { 
            PageNumber = pageNumber, 
            PageSize = pageSize 
        };
        var resultado = await _mediator.Send(query, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>
    /// Atualizar valor mensal do cliente
    /// </summary>
    [HttpPut("{id}/valor")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> AtualizarValor(
        long id,
        [FromBody] AtualizarValorRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AtualizarValorMensalClienteCommand(id, request.NovoValor);
        var sucesso = await _mediator.Send(command, cancellationToken);

        if (!sucesso)
            return NotFound($"Cliente com ID {id} não encontrado");

        return Ok();
    }

    /// <summary>
    /// Desativar cliente
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> Desativar(long id, CancellationToken cancellationToken)
    {
        var command = new DesativarClienteCommand(id);
        var sucesso = await _mediator.Send(command, cancellationToken);

        if (!sucesso)
            return NotFound($"Cliente com ID {id} não encontrado");

        return Ok();
    }
}

/// <summary>
/// Request para criar cliente
/// </summary>
public class CriarClienteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal ValorMensal { get; set; }
}

/// <summary>
/// Request para atualizar valor mensal
/// </summary>
public class AtualizarValorRequest
{
    public decimal NovoValor { get; set; }
}
