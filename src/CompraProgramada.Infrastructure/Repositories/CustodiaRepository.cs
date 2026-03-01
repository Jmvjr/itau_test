using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.ValueObjects;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class CustodiaRepository : BaseRepository<Custodia>, ICustodiaRepository
{
    private readonly AppDbContext _appContext; // Adicionar referência para joins complexos

    public CustodiaRepository(AppDbContext context) : base(context)
    {
        _appContext = context;
    }

    public async Task<Custodia?> ObterPorContaETickerAsync(long contaGraficaId, Ticker ticker)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ContaGraficaId == contaGraficaId && c.Ticker == ticker);
    }

    public async Task<IEnumerable<Custodia>> ObterPorContaAsync(long contaGraficaId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.ContaGraficaId == contaGraficaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Custodia>> ObterTodosPorClienteAsync(long clienteId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.ContaGrafica != null && c.ContaGrafica.ClienteId == clienteId)
            .ToListAsync();
    }
}
