using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.ValueObjects;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Cliente?> ObterPorCpfAsync(CPF cpf)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CPF == cpf);
    }

    public async Task<IEnumerable<Cliente>> ObterClientesAtivosAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.Ativo)
            .ToListAsync();
    }

    public async Task<bool> ExisteCpfAsync(CPF cpf)
    {
        return await _dbSet.AnyAsync(c => c.CPF == cpf);
    }
}
