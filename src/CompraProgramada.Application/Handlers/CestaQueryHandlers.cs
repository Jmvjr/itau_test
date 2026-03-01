using MediatR;
using CompraProgramada.Application.Queries;
using CompraProgramada.Application.DTOs;
using CompraProgramada.Application.Mappers;
using CompraProgramada.Domain.Interfaces;

namespace CompraProgramada.Application.Handlers;

/// <summary>
/// Handler para obter CestaRecomendacao por ID
/// </summary>
public class ObterCestaPorIdQueryHandler : IRequestHandler<ObterCestaPorIdQuery, CestaRecomendacaoDTO?>
{
    private readonly ICestaRepository _cestaRepository;

    public ObterCestaPorIdQueryHandler(ICestaRepository cestaRepository)
    {
        _cestaRepository = cestaRepository ?? throw new ArgumentNullException(nameof(cestaRepository));
    }

    public async Task<CestaRecomendacaoDTO?> Handle(ObterCestaPorIdQuery request, CancellationToken cancellationToken)
    {
        var cesta = await _cestaRepository.ObterPorIdAsync(request.CestaId);
        return cesta != null ? CestaMapper.ToDTO(cesta) : null;
    }
}

/// <summary>
/// Handler para listar cestas ativas
/// </summary>
public class ListarCestasAtivasQueryHandler : IRequestHandler<ListarCestasAtivasQuery, List<CestaRecomendacaoDTO>>
{
    private readonly ICestaRepository _cestaRepository;

    public ListarCestasAtivasQueryHandler(ICestaRepository cestaRepository)
    {
        _cestaRepository = cestaRepository ?? throw new ArgumentNullException(nameof(cestaRepository));
    }

    public async Task<List<CestaRecomendacaoDTO>> Handle(ListarCestasAtivasQuery request, CancellationToken cancellationToken)
    {
        var cestas = await _cestaRepository.ObterTodosAsync();
        var cestasAtivas = cestas.Where(c => c.Ativa).ToList();
        return cestasAtivas.Select(CestaMapper.ToDTO).ToList();
    }
}

/// <summary>
/// Handler para listar todas as cestas (ativas e inativas)
/// </summary>
public class ListarTodasAsCestasQueryHandler : IRequestHandler<ListarTodasAsCestasQuery, List<CestaRecomendacaoDTO>>
{
    private readonly ICestaRepository _cestaRepository;

    public ListarTodasAsCestasQueryHandler(ICestaRepository cestaRepository)
    {
        _cestaRepository = cestaRepository ?? throw new ArgumentNullException(nameof(cestaRepository));
    }

    public async Task<List<CestaRecomendacaoDTO>> Handle(ListarTodasAsCestasQuery request, CancellationToken cancellationToken)
    {
        var cestas = await _cestaRepository.ObterTodosAsync();
        return cestas.Select(CestaMapper.ToDTO).ToList();
    }
}

/// <summary>
/// Handler para obter a cesta ativa atualmente
/// </summary>
public class ObterCestaAtivaQueryHandler : IRequestHandler<ObterCestaAtivaQuery, CestaRecomendacaoDTO?>
{
    private readonly ICestaRepository _cestaRepository;

    public ObterCestaAtivaQueryHandler(ICestaRepository cestaRepository)
    {
        _cestaRepository = cestaRepository ?? throw new ArgumentNullException(nameof(cestaRepository));
    }

    public async Task<CestaRecomendacaoDTO?> Handle(ObterCestaAtivaQuery request, CancellationToken cancellationToken)
    {
        var cesta = await _cestaRepository.ObterAtivaAsync();
        return cesta != null ? CestaMapper.ToDTO(cesta) : null;
    }
}
