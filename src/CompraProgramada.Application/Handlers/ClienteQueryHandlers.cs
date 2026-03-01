using MediatR;
using CompraProgramada.Application.Queries;
using CompraProgramada.Application.DTOs;
using CompraProgramada.Application.Mappers;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Application.Handlers;

/// <summary>
/// Handler para obter Cliente por ID
/// </summary>
public class ObterClientePorIdQueryHandler : IRequestHandler<ObterClientePorIdQuery, ClienteDTO?>
{
    private readonly IClienteRepository _clienteRepository;

    public ObterClientePorIdQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDTO?> Handle(ObterClientePorIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
        return cliente != null ? ClienteMapper.ToDTO(cliente) : null;
    }
}

/// <summary>
/// Handler para listar clientes ativos
/// </summary>
public class ListarClientesAtivosQueryHandler : IRequestHandler<ListarClientesAtivosQuery, ListarClientesDTO>
{
    private readonly IClienteRepository _clienteRepository;

    public ListarClientesAtivosQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ListarClientesDTO> Handle(ListarClientesAtivosQuery request, CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObterClientesAtivosAsync();
        return ClienteMapper.ToListarDTO(clientes);
    }
}

/// <summary>
/// Handler para obter Cliente por CPF
/// </summary>
public class ObterClientePorCpfQueryHandler : IRequestHandler<ObterClientePorCpfQuery, ClienteDTO?>
{
    private readonly IClienteRepository _clienteRepository;

    public ObterClientePorCpfQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDTO?> Handle(ObterClientePorCpfQuery request, CancellationToken cancellationToken)
    {
        var cpf = new CPF(request.CPF);
        var cliente = await _clienteRepository.ObterPorCpfAsync(cpf);
        return cliente != null ? ClienteMapper.ToDTO(cliente) : null;
    }
}

/// <summary>
/// Handler para listar todos os clientes
/// </summary>
public class ListarTodosClientesQueryHandler : IRequestHandler<ListarTodosClientesQuery, ListarClientesDTO>
{
    private readonly IClienteRepository _clienteRepository;

    public ListarTodosClientesQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ListarClientesDTO> Handle(ListarTodosClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.ObterTodosAsync();
        return ClienteMapper.ToListarDTO(clientes);
    }
}
