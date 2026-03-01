using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class CestaRepository : BaseRepository<CestaRecomendacao>, ICestaRepository
{
    public CestaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<CestaRecomendacao?> ObterAtivaAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Ativa);
    }

    public async Task<IEnumerable<CestaRecomendacao>> ObterHistoricoAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync();
    }
}
