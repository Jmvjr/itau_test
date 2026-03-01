using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.ValueObjects;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class CotacaoRepository : BaseRepository<Cotacao>
{
    public CotacaoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Cotacao?> ObterMaisRecenteAsync(Ticker ticker)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.Ticker == ticker)
            .OrderByDescending(c => c.DataPregao)
            .FirstOrDefaultAsync();
    }

    public async Task<Cotacao?> ObterPorDataETickerAsync(DateTime data, Ticker ticker)
    {
        var dataStart = data.Date;
        var dataEnd = dataStart.AddDays(1);

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Ticker == ticker && 
                                      c.DataPregao >= dataStart && 
                                      c.DataPregao < dataEnd);
    }
}
