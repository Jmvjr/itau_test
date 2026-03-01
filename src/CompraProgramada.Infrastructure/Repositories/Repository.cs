using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Infrastructure.Data;

namespace CompraProgramada.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica de repositório para entidades sem repositório especializado
/// </summary>
public class Repository<T> : BaseRepository<T> where T : class
{
    public Repository(AppDbContext context) : base(context)
    {
    }
}
