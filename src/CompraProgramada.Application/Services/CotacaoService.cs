using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.ValueObjects;
using CompraProgramada.Domain.Interfaces;

namespace CompraProgramada.Application.Services;

/// <summary>
/// Serviço para gerenciar cotações do COTAHIST
/// </summary>
public interface ICotacaoService
{
    /// <summary>
    /// Importa cotações de um arquivo COTAHIST e salva no banco
    /// </summary>
    Task<int> ImportarCotahistAsync(string caminhoArquivo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a cotação mais recente de um ticker
    /// </summary>
    Task<Cotacao?> ObterCotacaoMaisRecenteAsync(string ticker, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a cotação de um ticker em uma data específica
    /// </summary>
    Task<Cotacao?> ObterCotacaoPorDataAsync(string ticker, DateTime data, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementação do serviço de cotações
/// </summary>
public class CotacaoService : ICotacaoService
{
    private readonly ICotacaoRepository _cotacaoRepository;

    public CotacaoService(ICotacaoRepository cotacaoRepository)
    {
        _cotacaoRepository = cotacaoRepository ?? throw new ArgumentNullException(nameof(cotacaoRepository));
    }

    /// <summary>
    /// Importa cotações de um arquivo COTAHIST
    /// </summary>
    public async Task<int> ImportarCotahistAsync(string caminhoArquivo, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException($"Arquivo COTAHIST não encontrado: {caminhoArquivo}");

        // Parsear arquivo
        var cotacoesDTO = CotahistParser.ParsearArquivo(caminhoArquivo);

        if (!cotacoesDTO.Any())
            return 0;

        int qtdImportada = 0;

        // Processar cada cotação do arquivo
        foreach (var cotacao in cotacoesDTO)
        {
            try
            {
                // Verificar se cotação já existe (por data + ticker)
                var existe = await _cotacaoRepository.ObterPorDataETickerAsync(
                    cotacao.DataPregao,
                    new Ticker(cotacao.Ticker));

                if (existe == null)
                {
                    // Criar nova cotação
                    var novaCotacao = new Cotacao(
                        ticker: new Ticker(cotacao.Ticker),
                        dataPregao: cotacao.DataPregao,
                        precoAbertura: cotacao.PrecoAbertura,
                        precoFechamento: cotacao.PrecoFechamento,
                        precoMaximo: cotacao.PrecoMaximo,
                        precoMinimo: cotacao.PrecoMinimo);

                    await _cotacaoRepository.AdicionarAsync(novaCotacao);
                    qtdImportada++;
                }
            }
            catch
            {
                // Continuar com próximas cotações se houver erro em uma específica
                continue;
            }
        }

        // Salvar todas as cotações de uma vez
        await _cotacaoRepository.SaveChangesAsync();

        return qtdImportada;
    }

    /// <summary>
    /// Obtém a cotação mais recente de um ticker
    /// </summary>
    public async Task<Cotacao?> ObterCotacaoMaisRecenteAsync(string ticker, CancellationToken cancellationToken = default)
    {
        var tickerVO = new Ticker(ticker);
        return await _cotacaoRepository.ObterMaisRecenteAsync(tickerVO);
    }

    /// <summary>
    /// Obtém a cotação de um ticker em uma data específica
    /// </summary>
    public async Task<Cotacao?> ObterCotacaoPorDataAsync(string ticker, DateTime data, CancellationToken cancellationToken = default)
    {
        var tickerVO = new Ticker(ticker);
        return await _cotacaoRepository.ObterPorDataETickerAsync(data, tickerVO);
    }
}
