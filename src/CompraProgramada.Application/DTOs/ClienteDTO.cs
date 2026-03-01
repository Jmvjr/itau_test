namespace CompraProgramada.Application.DTOs;

/// <summary>
/// DTO para Cliente - resposta (saída)
/// </summary>
public class ClienteDTO
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal ValorMensal { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataAdesao { get; set; }
}

/// <summary>
/// DTO para criar novo Cliente (entrada)
/// </summary>
public class CriarClienteDTO
{
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal ValorMensal { get; set; }
}

/// <summary>
/// DTO para atualizar valor mensal do Cliente
/// </summary>
public class AtualizarValorMensalClienteDTO
{
    public long ClienteId { get; set; }
    public decimal NovoValor { get; set; }
}

/// <summary>
/// DTO para listar clientes (resposta)
/// </summary>
public class ListarClientesDTO
{
    public int TotalClientes { get; set; }
    public int ClientesAtivos { get; set; }
    public int ClientesInativos { get; set; }
    public List<ClienteDTO> Clientes { get; set; } = new();
}
