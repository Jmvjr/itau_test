using Microsoft.AspNetCore.Mvc;
using CompraProgramada.Application.Services;

namespace CompraProgramada.API.Controllers;

/// <summary>
/// Controller para gerenciar Cotações
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CotacaoController : ControllerBase
{
    private readonly ICotacaoService _cotacaoService;
    private readonly ILogger<CotacaoController> _logger;

    public CotacaoController(ICotacaoService cotacaoService, ILogger<CotacaoController> logger)
    {
        _cotacaoService = cotacaoService ?? throw new ArgumentNullException(nameof(cotacaoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Importar cotações de um arquivo COTAHIST
    /// </summary>
    /// <remarks>
    /// O arquivo deve estar na pasta `cotacoes/` na raiz do projeto e ter o nome no padrão `COTAHIST_DYYYYMMDD.TXT`
    /// Exemplo: `COTAHIST_D20260225.TXT`
    /// </remarks>
    [HttpPost("importar-cotahist")]
    [ProducesResponseType(statusCode: 200, type: typeof(ImportarCotahistResponse))]
    [ProducesResponseType(statusCode: 400)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ImportarCotahist(
        [FromBody] ImportarCotahistRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Construir path do arquivo
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "cotacoes");
            var caminhoArquivo = Path.Combine(baseDir, request.NomeArquivo);

            _logger.LogInformation("Iniciando importação de cotações do arquivo: {NomeArquivo}", request.NomeArquivo);

            // Importar cotações
            var qtdImportada = await _cotacaoService.ImportarCotahistAsync(caminhoArquivo, cancellationToken);

            _logger.LogInformation("Importação concluída: {QtdImportada} cotações", qtdImportada);

            return Ok(new ImportarCotahistResponse
            {
                Sucesso = true,
                QtdImportada = qtdImportada,
                Mensagem = $"{qtdImportada} cotações importadas com sucesso"
            });
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "Arquivo COTAHIST não encontrado");
            return BadRequest(new ImportarCotahistResponse
            {
                Sucesso = false,
                QtdImportada = 0,
                Mensagem = $"Arquivo não encontrado: {ex.Message}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao importar cotações COTAHIST");
            return StatusCode(500, new ImportarCotahistResponse
            {
                Sucesso = false,
                QtdImportada = 0,
                Mensagem = $"Erro ao importar: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Obter cotação mais recente de um ticker
    /// </summary>
    [HttpGet("{ticker}/mais-recente")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ObterMaisRecente(string ticker, CancellationToken cancellationToken)
    {
        var cotacao = await _cotacaoService.ObterCotacaoMaisRecenteAsync(ticker, cancellationToken);

        if (cotacao is null)
            return NotFound($"Nenhuma cotação encontrada para {ticker}");

        return Ok(new
        {
            ticker = cotacao.Ticker.Valor,
            dataPregao = cotacao.DataPregao,
            precoAbertura = cotacao.PrecoAbertura,
            precoFechamento = cotacao.PrecoFechamento,
            precoMaximo = cotacao.PrecoMaximo,
            precoMinimo = cotacao.PrecoMinimo
        });
    }

    /// <summary>
    /// Obter cotação de um ticker em uma data específica
    /// </summary>
    [HttpGet("{ticker}/data/{data}")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 404)]
    [ProducesResponseType(statusCode: 500)]
    public async Task<IActionResult> ObterPorData(string ticker, string data, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParseExact(data, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dataParsed))
            return BadRequest("Data deve estar no formato YYYYMMDD");

        var cotacao = await _cotacaoService.ObterCotacaoPorDataAsync(ticker, dataParsed, cancellationToken);

        if (cotacao is null)
            return NotFound($"Nenhuma cotação encontrada para {ticker} na data {data}");

        return Ok(new
        {
            ticker = cotacao.Ticker.Valor,
            dataPregao = cotacao.DataPregao,
            precoAbertura = cotacao.PrecoAbertura,
            precoFechamento = cotacao.PrecoFechamento,
            precoMaximo = cotacao.PrecoMaximo,
            precoMinimo = cotacao.PrecoMinimo
        });
    }
}

/// <summary>
/// Request para importar COTAHIST
/// </summary>
public class ImportarCotahistRequest
{
    public string NomeArquivo { get; set; } = string.Empty;
}

/// <summary>
/// Response ao importar COTAHIST
/// </summary>
public class ImportarCotahistResponse
{
    public bool Sucesso { get; set; }
    public int QtdImportada { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
