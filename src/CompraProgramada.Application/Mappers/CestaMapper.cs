using CompraProgramada.Domain.Entities;
using CompraProgramada.Application.DTOs;

namespace CompraProgramada.Application.Mappers;

/// <summary>
/// Mapper para conversão entre CestaRecomendacao (domain) e CestaRecomendacaoDTO
/// </summary>
public static class CestaMapper
{
    public static CestaRecomendacaoDTO ToDTO(CestaRecomendacao cesta)
    {
        return new CestaRecomendacaoDTO
        {
            Id = cesta.Id,
            Nome = cesta.Nome,
            Ativa = cesta.Ativa,
            DataCriacao = cesta.DataCriacao,
            DataDesativacao = cesta.DataDesativacao,
            Itens = cesta.Itens.Select(item => new ItemCestaDTO
            {
                Ticker = item.Ticker.Valor,
                Percentual = item.Percentual.Valor
            }).ToList()
        };
    }

    public static List<CestaRecomendacaoDTO> ToListaDTOs(IEnumerable<CestaRecomendacao> cestas)
    {
        return cestas.Select(ToDTO).ToList();
    }
}
