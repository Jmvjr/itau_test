using MediatR;
using CompraProgramada.Application.Commands;
using CompraProgramada.Application.DTOs;
using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Application.Handlers;

/// <summary>
/// Handler para criar um novo Cliente
/// </summary>
public class CriarClienteCommandHandler : IRequestHandler<CriarClienteCommand, long>
{
    private readonly IClienteRepository _clienteRepository;

    public CriarClienteCommandHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<long> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        // Converter string CPF para ValueObject
        var cpf = new CPF(request.CPF);

        // Validar se CPF já existe
        var cpfJaExiste = await _clienteRepository.ExisteCpfAsync(cpf);
        if (cpfJaExiste)
            throw new InvalidOperationException($"CPF {request.CPF} já cadastrado no sistema");

        // Criar novo cliente (entidade de domínio)
        var cliente = new Cliente(
            nome: request.Nome,
            cpf: request.CPF,
            email: request.Email,
            valorMensal: request.ValorMensal);

        // Persistir
        await _clienteRepository.AdicionarAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return cliente.Id;
    }
}

/// <summary>
/// Handler para desativar Cliente
/// </summary>
public class DesativarClienteCommandHandler : IRequestHandler<DesativarClienteCommand, bool>
{
    private readonly IClienteRepository _clienteRepository;

    public DesativarClienteCommandHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<bool> Handle(DesativarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
        if (cliente == null)
            throw new InvalidOperationException($"Cliente {request.ClienteId} não encontrado");

        cliente.Sair();
        _clienteRepository.Atualizar(cliente);
        await _clienteRepository.SaveChangesAsync();

        return true;
    }
}

/// <summary>
/// Handler para atualizar valor mensal do Cliente
/// </summary>
public class AtualizarValorMensalClienteCommandHandler : IRequestHandler<AtualizarValorMensalClienteCommand, bool>
{
    private readonly IClienteRepository _clienteRepository;

    public AtualizarValorMensalClienteCommandHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<bool> Handle(AtualizarValorMensalClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
        if (cliente == null)
            throw new InvalidOperationException($"Cliente {request.ClienteId} não encontrado");

        cliente.AlterarValorMensal(request.NovoValor);
        _clienteRepository.Atualizar(cliente);
        await _clienteRepository.SaveChangesAsync();

        return true;
    }
}
