using CompraProgramada.Domain.Entities;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class DistribuicaoRepository : BaseRepository<Distribuicao>
{
    public DistribuicaoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Distribuicao>> ObterPorOrdemCompraAsync(long ordemCompraId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(d => d.OrdemCompraId == ordemCompraId)
            .OrderByDescending(d => d.DataDistribuicao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Distribuicao>> ObterPorClienteAsync(long clienteId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(d => d.ClienteId == clienteId)
            .OrderByDescending(d => d.DataDistribuicao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Distribuicao>> ObterPorDataAsync(DateTime data)
    {
        var dataStart = data.Date;
        var dataEnd = dataStart.AddDays(1);

        return await _dbSet
            .AsNoTracking()
            .Where(d => d.DataDistribuicao >= dataStart && d.DataDistribuicao < dataEnd)
            .OrderByDescending(d => d.DataDistribuicao)
            .ToListAsync();
    }
}
