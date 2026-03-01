using MediatR;

namespace CompraProgramada.Application.Commands;

/// <summary>
/// Comando para criar um novo Cliente
/// RN-001 a RN-013
/// </summary>
public class CriarClienteCommand : IRequest<long>
{
    public string Nome { get; set; }
    public string CPF { get; set; }
    public string Email { get; set; }
    public decimal ValorMensal { get; set; }

    public CriarClienteCommand(string nome, string cpf, string email, decimal valorMensal)
    {
        Nome = nome;
        CPF = cpf;
        Email = email;
        ValorMensal = valorMensal;
    }
}

/// <summary>
/// Comando para desativar um Cliente
/// </summary>
public class DesativarClienteCommand : IRequest<bool>
{
    public long ClienteId { get; set; }

    public DesativarClienteCommand(long clienteId)
    {
        ClienteId = clienteId;
    }
}

/// <summary>
/// Comando para atualizar valor mensal do Cliente
/// </summary>
public class AtualizarValorMensalClienteCommand : IRequest<bool>
{
    public long ClienteId { get; set; }
    public decimal NovoValor { get; set; }

    public AtualizarValorMensalClienteCommand(long clienteId, decimal novoValor)
    {
        ClienteId = clienteId;
        NovoValor = novoValor;
    }
}
