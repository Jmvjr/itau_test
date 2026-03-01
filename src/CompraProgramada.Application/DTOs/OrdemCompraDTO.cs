namespace CompraProgramada.Application.DTOs;

/// <summary>
/// DTO para OrdemCompra - resposta
/// </summary>
public class OrdemCompraDTO
{
    public long Id { get; set; }
    public long ContaMasterId { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string TipoMercado { get; set; } = string.Empty;
    public DateTime DataOrdem { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO para criar OrdemCompra (entrada)
/// </summary>
public class CriarOrdemCompraDTO
{
    public long ContaMasterId { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string TipoMercado { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resultado de execução de compra
/// </summary>
public class ResultadoCompraDTO
{
    public long OrdemId { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
