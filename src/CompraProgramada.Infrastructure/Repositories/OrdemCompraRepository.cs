using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class OrdemCompraRepository : BaseRepository<OrdemCompra>, IOrdemCompraRepository
{
    public OrdemCompraRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<OrdemCompra>> ObterPorDataAsync(DateTime data)
    {
        var dataStart = data.Date;
        var dataEnd = dataStart.AddDays(1);

        return await _dbSet
            .AsNoTracking()
            .Where(oc => oc.DataExecucao >= dataStart && oc.DataExecucao < dataEnd)
            .OrderBy(oc => oc.DataExecucao)
            .ToListAsync();
    }
}
