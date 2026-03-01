using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class RebalanceamentoRepository : BaseRepository<Rebalanceamento>, IRebalanceamentoRepository
{
    public RebalanceamentoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rebalanceamento>> ObterPorClienteAsync(long clienteId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.DataRebalanceamento)
            .ToListAsync();
    }
}
