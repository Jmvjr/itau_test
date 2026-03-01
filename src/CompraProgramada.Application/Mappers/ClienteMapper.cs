using CompraProgramada.Domain.Entities;
using CompraProgramada.Application.DTOs;

namespace CompraProgramada.Application.Mappers;

/// <summary>
/// Mapper para conversão entre Cliente (domain) e ClienteDTO
/// </summary>
public static class ClienteMapper
{
    public static ClienteDTO ToDTO(Cliente cliente)
    {
        return new ClienteDTO
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CPF = cliente.CPF.ToString(),
            Email = cliente.Email,
            ValorMensal = cliente.ValorMensal,
            Ativo = cliente.Ativo,
            DataAdesao = cliente.DataAdesao
        };
    }

    public static ListarClientesDTO ToListarDTO(IEnumerable<Cliente> clientes)
    {
        var clientesList = clientes.ToList();
        var dtos = clientesList.Select(ToDTO).ToList();
        
        return new ListarClientesDTO
        {
            TotalClientes = clientesList.Count,
            ClientesAtivos = clientesList.Count(c => c.Ativo),
            ClientesInativos = clientesList.Count(c => !c.Ativo),
            Clientes = dtos
        };
    }
}
