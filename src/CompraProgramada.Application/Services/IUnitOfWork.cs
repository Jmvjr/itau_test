using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.Entities;

namespace CompraProgramada.Application.Services;

/// <summary>
/// Unit of Work — Padrão para gerenciar transações e repositórios
/// Garante que todas as mudanças sejam persistidas juntas ou nenhuma é persistida
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<Cliente> ClienteRepository { get; }
    IRepository<CestaRecomendacao> CestaRepository { get; }
    IRepository<OrdemCompra> OrdemCompraRepository { get; }
    IRepository<Cotacao> CotacaoRepository { get; }
    IRepository<ContaGrafica> ContaGraficaRepository { get; }
    IRepository<Custodia> CustodiaRepository { get; }
    IRepository<Distribuicao> DistribuicaoRepository { get; }
    IRepository<EventoIR> EventoIRRepository { get; }
    IRepository<Rebalanceamento> RebalanceamentoRepository { get; }

    Task SaveChangesAsync();
}
