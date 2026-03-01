using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Infrastructure.Data;
using CompraProgramada.Infrastructure.Repositories;
using CompraProgramada.Application.Services;

namespace CompraProgramada.Infrastructure;

/// <summary>
/// Implementação do padrão Unit of Work
/// Centraliza o acesso a todos os repositórios e gerencia transações
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<Cliente> ClienteRepository { get; private set; }
    public IRepository<CestaRecomendacao> CestaRepository { get; private set; }
    public IRepository<OrdemCompra> OrdemCompraRepository { get; private set; }
    public IRepository<Cotacao> CotacaoRepository { get; private set; }
    public IRepository<ContaGrafica> ContaGraficaRepository { get; private set; }
    public IRepository<Custodia> CustodiaRepository { get; private set; }
    public IRepository<Distribuicao> DistribuicaoRepository { get; private set; }
    public IRepository<EventoIR> EventoIRRepository { get; private set; }
    public IRepository<Rebalanceamento> RebalanceamentoRepository { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        // Inicializar repositórios
        ClienteRepository = new ClienteRepository(context);
        CestaRepository = new Repository<CestaRecomendacao>(context);
        OrdemCompraRepository = new Repository<OrdemCompra>(context);
        CotacaoRepository = new CotacaoRepository(context);
        ContaGraficaRepository = new ContaGraficaRepository(context);
        CustodiaRepository = new Repository<Custodia>(context);
        DistribuicaoRepository = new Repository<Distribuicao>(context);
        EventoIRRepository = new Repository<EventoIR>(context);
        RebalanceamentoRepository = new Repository<Rebalanceamento>(context);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
