using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Enums;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.ValueObjects;
using CompraProgramada.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Repositories;

public class ContaGraficaRepository : BaseRepository<ContaGrafica>, IContaGraficaRepository
{
    public ContaGraficaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ContaGrafica?> ObterPorNumeroAsync(string numero)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(cg => cg.NumeroConta == numero);
    }

    public async Task<ContaGrafica?> ObterMasterAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(cg => cg.Tipo == TipoConta.Master);
    }

    public async Task<ContaGrafica?> ObterFilhoteDoClienteAsync(long clienteId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(cg => cg.ClienteId == clienteId && cg.Tipo == TipoConta.Filhote);
    }
}
