using MediatR;
using CompraProgramada.Application.DTOs;

namespace CompraProgramada.Application.Queries;

/// <summary>
/// Query para obter um Cliente por ID
/// </summary>
public class ObterClientePorIdQuery : IRequest<ClienteDTO?>
{
    public long ClienteId { get; set; }

    public ObterClientePorIdQuery(long clienteId)
    {
        ClienteId = clienteId;
    }
}

/// <summary>
/// Query para listar todos os clientes ativos
/// </summary>
public class ListarClientesAtivosQuery : IRequest<ListarClientesDTO>
{
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}

/// <summary>
/// Query para obter cliente por CPF
/// </summary>
public class ObterClientePorCpfQuery : IRequest<ClienteDTO?>
{
    public string CPF { get; set; }

    public ObterClientePorCpfQuery(string cpf)
    {
        CPF = cpf;
    }
}

/// <summary>
/// Query para listar todos os clientes (ativos e inativos)
/// </summary>
public class ListarTodosClientesQuery : IRequest<ListarClientesDTO>
{
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}
