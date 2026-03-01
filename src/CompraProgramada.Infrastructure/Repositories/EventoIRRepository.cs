using CompraProgramada.Domain.Entities;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class EventoIRRepository : BaseRepository<EventoIR>
{
    public EventoIRRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EventoIR>> ObterNaoPublicadosAsync()
    {
        // Implementar lógica para obter eventos não publicados
        // Por enquanto, retorna eventos recentes
        return await _dbSet
            .AsNoTracking()
            .OrderByDescending(e => e.DataEvento)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventoIR>> ObterPorClienteAsync(long clienteId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.ClienteId == clienteId)
            .OrderByDescending(e => e.DataEvento)
            .ToListAsync();
    }
}
