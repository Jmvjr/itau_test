namespace CompraProgramada.Application.DTOs;

/// <summary>
/// DTO para ItemCesta (ativo dentro de uma cesta)
/// </summary>
public class ItemCestaDTO
{
    public string Ticker { get; set; } = string.Empty;
    public decimal Percentual { get; set; }
}

/// <summary>
/// DTO para CestaRecomendacao - resposta
/// </summary>
public class CestaRecomendacaoDTO
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataDesativacao { get; set; }
    public List<ItemCestaDTO> Itens { get; set; } = new();
}

/// <summary>
/// DTO para criar nova CestaRecomendacao
/// </summary>
public class CriarCestaRecomendacaoDTO
{
    public string Nome { get; set; } = string.Empty;
    public List<ItemCestaDTO> Itens { get; set; } = new();
}

/// <summary>
/// DTO para resposta de cesta ativa
/// </summary>
public class ObterCestaAtivADTO
{
    public long CestaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<ItemCestaDTO> Itens { get; set; } = new();
}
