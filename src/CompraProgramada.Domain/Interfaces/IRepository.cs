namespace CompraProgramada.Domain.Interfaces;

/// <summary>
/// Interface base para todos os repositórios
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> ObterPorIdAsync(long id);
    Task<IEnumerable<T>> ObterTodosAsync();
    Task AdicionarAsync(T entidade);
    void Atualizar(T entidade);
    void Remover(T entidade);
    Task SaveChangesAsync();
}

/// <summary>
/// Repositório de Cliente
/// </summary>
public interface IClienteRepository : IRepository<Entities.Cliente>
{
    Task<Entities.Cliente?> ObterPorCpfAsync(string cpf);
    Task<IEnumerable<Entities.Cliente>> ObterClientesAtivosAsync();
    Task<bool> ExisteCpfAsync(string cpf);
}

/// <summary>
/// Repositório de ContaGrafica
/// </summary>
public interface IContaGraficaRepository : IRepository<Entities.ContaGrafica>
{
    Task<Entities.ContaGrafica?> ObterPorNumeroAsync(string numero);
    Task<Entities.ContaGrafica?> ObterMasterAsync();
    Task<Entities.ContaGrafica?> ObterFilhoteDoClienteAsync(long clienteId);
}

/// <summary>
/// Repositório de Custodia
/// </summary>
public interface ICustodiaRepository : IRepository<Entities.Custodia>
{
    Task<Entities.Custodia?> ObterPorContaETickerAsync(long contaGraficaId, string ticker);
    Task<IEnumerable<Entities.Custodia>> ObterPorContaAsync(long contaGraficaId);
}

/// <summary>
/// Repositório de CestaRecomendacao
/// </summary>
public interface ICestaRepository : IRepository<Entities.CestaRecomendacao>
{
    Task<Entities.CestaRecomendacao?> ObterAtivaAsync();
    Task<IEnumerable<Entities.CestaRecomendacao>> ObterHistoricoAsync();
}

/// <summary>
/// Repositório de OrdemCompra
/// </summary>
public interface IOrdemCompraRepository : IRepository<Entities.OrdemCompra>
{
    Task<IEnumerable<Entities.OrdemCompra>> ObterPorDataAsync(DateTime data);
}

/// <summary>
/// Repositório de Cotacao
/// </summary>
public interface ICotacaoRepository : IRepository<Entities.Cotacao>
{
    Task<Entities.Cotacao?> ObterMaisRecenteAsync(string ticker);
    Task<Entities.Cotacao?> ObterPorDataETickerAsync(DateTime data, string ticker);
}

/// <summary>
/// Repositório de EventoIR
/// </summary>
public interface IEventoIRRepository : IRepository<Entities.EventoIR>
{
    Task<IEnumerable<Entities.EventoIR>> ObterNaoPublicadosAsync();
    Task<IEnumerable<Entities.EventoIR>> ObterPorClienteAsync(long clienteId);
}

/// <summary>
/// Repositório de Rebalanceamento
/// </summary>
public interface IRebalanceamentoRepository : IRepository<Entities.Rebalanceamento>
{
    Task<IEnumerable<Entities.Rebalanceamento>> ObterPorClienteAsync(long clienteId);
}
