using MediatR;
using CompraProgramada.Application.DTOs;

namespace CompraProgramada.Application.Queries;

/// <summary>
/// Query para obter uma CestaRecomendacao por ID
/// </summary>
public class ObterCestaPorIdQuery : IRequest<CestaRecomendacaoDTO?>
{
    public long CestaId { get; set; }

    public ObterCestaPorIdQuery(long cestaId)
    {
        CestaId = cestaId;
    }
}

/// <summary>
/// Query para listar todas as cestas ativas
/// </summary>
public class ListarCestasAtivasQuery : IRequest<List<CestaRecomendacaoDTO>>
{
}

/// <summary>
/// Query para listar todas as cestas (ativas e inativas)
/// </summary>
public class ListarTodasAsCestasQuery : IRequest<List<CestaRecomendacaoDTO>>
{
}

/// <summary>
/// Query para obter a cesta ativa atualmente
/// </summary>
public class ObterCestaAtivaQuery : IRequest<CestaRecomendacaoDTO?>
{
}
